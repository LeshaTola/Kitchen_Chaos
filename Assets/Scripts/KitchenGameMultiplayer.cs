using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
	public static int MAX_CLIENT_COUNT = 4;

	public event Action OnTryToConnecting;
	public event Action OnFailToConnect;

	[SerializeField] private KitchenObjectListSO kitchenObjectListSO;

	public static KitchenGameMultiplayer Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void StartHost()
	{
		NetworkManager.Singleton.StartHost();
		NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCollback;
	}

	public void StartClient()
	{
		OnTryToConnecting?.Invoke();

		NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
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
		Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefabs);

		var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
		kitchenObjectNetworkObject.Spawn();

		var kitchenObjectParentInterface = GetKitchenObjectParentFromNetworkObjectReference(kitchenObjectParentNetworkObjectReference);

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

	private void OnClientDisconnectCallback(ulong clientId)
	{
		OnFailToConnect?.Invoke();
	}
}
