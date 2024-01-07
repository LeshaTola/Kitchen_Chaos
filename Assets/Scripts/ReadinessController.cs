using System;
using System.Collections.Generic;
using Unity.Netcode;

public class ReadinessController : NetworkBehaviour
{
	public event Action OnPlayerSetReady;

	private Dictionary<ulong, bool> playerReadyDictionary;

	public static ReadinessController Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		playerReadyDictionary = new();
	}

	public void SetReadiness()
	{
		SetReadinessServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetReadinessServerRpc(ServerRpcParams serverRpcParams = default)
	{
		SetReadyClientRpc(serverRpcParams.Receive.SenderClientId);

		bool allClientsReady = true;
		foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
			{
				allClientsReady = false;
				break;
			}
		}

		if (allClientsReady)
		{
			GameLobby.Instance.DeleteLobby();
			Loader.LoadNetwork(Loader.Scene.GameScene);
		}
	}

	[ClientRpc]
	private void SetReadyClientRpc(ulong clientId)
	{
		playerReadyDictionary[clientId] = true;
		OnPlayerSetReady?.Invoke();
	}

	public bool IsPlayerReady(ulong playerId)
	{
		return playerReadyDictionary.ContainsKey(playerId) && playerReadyDictionary[playerId];
	}
}
