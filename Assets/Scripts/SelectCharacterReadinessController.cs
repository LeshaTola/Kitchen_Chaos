using System.Collections.Generic;
using Unity.Netcode;

public class SelectCharacterReadinessController : NetworkBehaviour
{

	private Dictionary<ulong, bool> playerReadyDictionary;

	public static SelectCharacterReadinessController Instance { get; private set; }

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
		playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

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
			Loader.LoadNetwork(Loader.Scene.GameScene);
		}
	}
}
