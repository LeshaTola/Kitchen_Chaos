using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
	public static int MAX_CLIENT_COUNT = 4;
	public static string PLAYER_NAME_PLAYERPREFS_KEY = "PlayerNameKey";

	public event Action OnTryToConnecting;
	public event Action OnFailToConnect;
	public event Action OnPlayerDataNetworkListChanged;

	[SerializeField] private KitchenObjectListSO kitchenObjectListSO;
	[SerializeField] private List<Color> playerColorsList;

	private NetworkList<PlayerData> playerDataNetworkList;

	private string playerName;
	public string PlayerName
	{
		get { return playerName; }
		set
		{
			playerName = value;
			PlayerPrefs.SetString(PLAYER_NAME_PLAYERPREFS_KEY, value);
		}
	}

	public static KitchenGameMultiplayer Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);

		playerName = PlayerPrefs.GetString(PLAYER_NAME_PLAYERPREFS_KEY, $"Player{UnityEngine.Random.Range(0, 100)}");

		playerDataNetworkList = new NetworkList<PlayerData>();
		playerDataNetworkList.OnListChanged += PlayerDataNetworkListChanged;
	}

	private void PlayerDataNetworkListChanged(NetworkListEvent<PlayerData> changeEvent)
	{
		OnPlayerDataNetworkListChanged?.Invoke();
	}

	public void StartHost()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCollback;

		NetworkManager.Singleton.OnClientConnectedCallback += Server_OnClientConnectedCallback;
		NetworkManager.Singleton.OnClientDisconnectCallback += Server_OnClientDisconnectCallback;

		NetworkManager.Singleton.StartHost();
	}

	public void StartClient()
	{
		OnTryToConnecting?.Invoke();

		NetworkManager.Singleton.OnClientDisconnectCallback += Client_OnClientDisconnectCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += Client_OnClientConnectedCallback;
		NetworkManager.Singleton.StartClient();
	}

	public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
	{
		SpawnKitchenObjectServerRpc(GetIndexOfKitchenObject(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
		var kitchenObjectSO = GetKitchenObject(kitchenObjectSOIndex);

		var kitchenObjectParentInterface = GetKitchenObjectParentFromNetworkObjectReference(kitchenObjectParentNetworkObjectReference);

		if (kitchenObjectParentInterface.HasKitchenObject())
		{
			return;
		}

		Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefabs);
		var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
		kitchenObjectNetworkObject.Spawn();


		kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(kitchenObjectParentInterface);
	}

	public void DestroyKitchenObject(KitchenObject kitchenObject)
	{
		DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
	}

	[ServerRpc(RequireOwnership = false)]
	private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);

		if (kitchenObjectNetworkObject == null)
		{
			return;
		}

		var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

		kitchenObject.DestroySelf();
		ClearKitchenObjectForParentClientRpc(kitchenObjectNetworkObjectReference);
	}

	[ClientRpc]
	private void ClearKitchenObjectForParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
		var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

		kitchenObject.ClearKitchenObjectForParent();
	}

	public KitchenObjectSO GetKitchenObject(int KitchenObjectIndex)
	{
		return kitchenObjectListSO.List[KitchenObjectIndex];
	}

	public bool IsPlayerIndexConnected(int playerIndex)
	{
		return playerIndex < playerDataNetworkList.Count;
	}

	public int GetIndexOfKitchenObject(KitchenObjectSO kitchenObjectSO)
	{
		return kitchenObjectListSO.List.IndexOf(kitchenObjectSO);
	}

	private IKitchenObjectParent GetKitchenObjectParentFromNetworkObjectReference(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
		kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
		return kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
	}

	private void ConnectionApprovalCollback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		if (!SceneManager.GetActiveScene().name.Equals(Loader.Scene.SelectCharacterScene.ToString()))
		{
			response.Approved = false;
			response.Reason = "Game is already started";
			return;
		}

		if (NetworkManager.ConnectedClients.Count == MAX_CLIENT_COUNT)
		{
			response.Approved = false;
			response.Reason = "Game is full";
			return;
		}

		response.Approved = true;
	}

	private void Client_OnClientDisconnectCallback(ulong clientId)
	{
		OnFailToConnect?.Invoke();
	}

	private void Server_OnClientDisconnectCallback(ulong clientId)
	{
		if (NetworkManager.ShutdownInProgress)
		{
			return;
		}

		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			if (playerDataNetworkList[i].clientId == clientId)
			{
				playerDataNetworkList.RemoveAt(i);
			}
		}
	}

	private void Server_OnClientConnectedCallback(ulong clientId)
	{
		playerDataNetworkList.Add(new PlayerData
		{
			clientId = clientId,
			colorIndex = GetFirstUnusedColorIndex(),
		});

		ChangeNameServerRpc(PlayerName);
		ChangeIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	private void Client_OnClientConnectedCallback(ulong clientId)
	{
		ChangeNameServerRpc(PlayerName);
		ChangeIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void ChangeIdServerRpc(string id, ServerRpcParams serverRpcParams = default)
	{

		var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

		var playerData = playerDataNetworkList[playerDataIndex];
		playerData.id = id;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	[ServerRpc(RequireOwnership = false)]
	private void ChangeNameServerRpc(string name, ServerRpcParams serverRpcParams = default)
	{

		var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

		var playerData = playerDataNetworkList[playerDataIndex];
		playerData.name = name;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	public PlayerData GetPlayerDataFromIndex(int index)
	{
		return playerDataNetworkList[index];
	}

	public PlayerData GetPlayerDataFromClientId(ulong clientId)
	{
		foreach (var playerData in playerDataNetworkList)
		{
			if (playerData.clientId == clientId)
			{
				return playerData;
			}
		}
		return default;
	}

	public int GetPlayerDataIndexFromClientId(ulong clientId)
	{
		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			if (playerDataNetworkList[i].clientId == clientId)
			{
				return i;
			}
		}
		return default;
	}

	public PlayerData GetPlayerData()
	{
		return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
	}

	public Color GetColor(int colorIndex)
	{
		return playerColorsList[colorIndex];
	}

	public void ChangePlayerColor(int colorIndex)
	{
		ChangeColorServerRpc(colorIndex);
	}

	[ServerRpc(RequireOwnership = false)]
	private void ChangeColorServerRpc(int colorIndex, ServerRpcParams serverRpcParams = default)
	{
		if (!IsColorAvailable(colorIndex))
		{
			return;
		}

		var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

		var playerData = playerDataNetworkList[playerDataIndex];
		playerData.colorIndex = colorIndex;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	private bool IsColorAvailable(int colorIndex)
	{
		foreach (var playerData in playerDataNetworkList)
		{
			if (playerData.colorIndex == colorIndex)
			{
				return false;
			}
		}
		return true;
	}

	private int GetFirstUnusedColorIndex()
	{
		for (int i = 0; i < playerColorsList.Count; i++)
		{
			if (IsColorAvailable(i))
			{
				return i;
			}
		}

		return 0;
	}

	public void DisconnectClient(ulong clientId)
	{
		NetworkManager.DisconnectClient(clientId);
		Server_OnClientDisconnectCallback(clientId);
	}
}
