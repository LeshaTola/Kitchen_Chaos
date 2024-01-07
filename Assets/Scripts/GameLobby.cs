using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour
{
	public const string RELAY_JOIN_CODE_KEY = "RelayJoinCodeKey";

	public event Action OnLobbyCreateStarted;
	public event Action OnCreateLobbyFailed;
	public event Action OnJoinLobbyStarted;
	public event Action OnJoinLobbyFailed;
	public event Action OnQuickJoinLobbyFailed;

	public event Action<List<Lobby>> OnRefreshLobbies;

	public Lobby JoinedLobby { get; private set; }

	public static GameLobby Instance { get; private set; }

	private float heartBeatTimer = 15f;
	private float RefreshTimer = 3f;

	private void Awake()
	{
		Instance = this;
		InitUnityAuthentication();
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		HandleHeartBeat();
		HandlePeriodicallyLobbyRefresh();
	}

	private async Task<Allocation> AllocateRelay()
	{
		try
		{
			return await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_CLIENT_COUNT - 1);
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e.Message);
			return default;
		}
	}

	private async Task<string> GetRelayJoinCode(Allocation allocation)
	{
		try
		{
			return await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e.Message);
			return default;
		}
	}

	private async Task<JoinAllocation> JoinRelay(string joinCode)
	{
		try
		{
			return await RelayService.Instance.JoinAllocationAsync(joinCode);
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e.Message);
			return default;
		}
	}

	public async void CreateLobby(string lobbyName, bool isPrivate)
	{
		OnLobbyCreateStarted?.Invoke();
		try
		{
			JoinedLobby = await LobbyService.Instance.CreateLobbyAsync(
				lobbyName,
				KitchenGameMultiplayer.MAX_CLIENT_COUNT,
				new CreateLobbyOptions { IsPrivate = isPrivate });

			var allocation = await AllocateRelay();
			string relayJoinCode = await GetRelayJoinCode(allocation);

			await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions()
			{
				Data = new()
				{
					{RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
				}
			});

			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

			KitchenGameMultiplayer.Instance.StartHost();
			Loader.LoadNetwork(Loader.Scene.SelectCharacterScene);
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
			OnCreateLobbyFailed?.Invoke();
		}
	}

	private async Task PrepareBeforeStartClient()
	{
		string relayJoinCode = JoinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;

		var joinAllocation = await JoinRelay(relayJoinCode);

		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
	}

	public async void JoinLobbyByCode(string lobbyCode)
	{
		OnJoinLobbyStarted?.Invoke();
		try
		{
			JoinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

			await PrepareBeforeStartClient();
			KitchenGameMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException e)
		{
			OnJoinLobbyFailed?.Invoke();
			Debug.Log(e);
		}
	}

	public async void JoinLobbyById(string lobbyId)
	{
		OnJoinLobbyStarted?.Invoke();
		try
		{
			JoinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

			await PrepareBeforeStartClient();
			KitchenGameMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException e)
		{
			OnJoinLobbyFailed?.Invoke();
			Debug.Log(e);
		}
	}

	public async void QuickJoin()
	{
		OnJoinLobbyStarted?.Invoke();
		try
		{
			JoinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

			await PrepareBeforeStartClient();
			KitchenGameMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException e)
		{
			OnQuickJoinLobbyFailed?.Invoke();
			Debug.Log(e);
		}
	}

	public async void DeleteLobby()
	{
		try
		{
			if (JoinedLobby != null)
			{
				await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);
			}
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}

	public async void LeaveLobby()
	{
		try
		{
			if (JoinedLobby != null)
			{
				await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);

				JoinedLobby = null;
			}
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}

	public async void KickPlayer(string playerId)
	{
		try
		{
			if (IsLobbyHost())
			{
				await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerId);
			}
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}

	private void HandleHeartBeat()
	{
		if (IsLobbyHost())
		{
			heartBeatTimer -= Time.deltaTime;
			if (heartBeatTimer < 0)
			{
				float heartBeatTimerMax = 15f;
				heartBeatTimer = heartBeatTimerMax;
				LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id);
			}
		}
	}

	private bool IsLobbyHost()
	{
		return JoinedLobby != null && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;
	}

	private async void InitUnityAuthentication()
	{
		try
		{
			if (UnityServices.State != ServicesInitializationState.Initialized)
			{
				var initOptions = new InitializationOptions();
				initOptions.SetProfile(UnityEngine.Random.Range(100, 1000).ToString());

				await UnityServices.InitializeAsync(initOptions);

				await AuthenticationService.Instance.SignInAnonymouslyAsync();
			}
		}
		catch (AuthenticationException e)
		{
			Debug.Log(e);
		}
	}


	private void HandlePeriodicallyLobbyRefresh()
	{
		if (JoinedLobby == null
			&& AuthenticationService.Instance.IsSignedIn
			&& SceneManager.GetActiveScene().name.Equals(Loader.Scene.LobbyScene.ToString()))
		{
			RefreshTimer -= Time.deltaTime;
			if (RefreshTimer <= 0)
			{
				float RefreshTimerMax = 3f;
				RefreshTimer = RefreshTimerMax;
				GetListOfLobbies();
			}
		}
	}

	private async void GetListOfLobbies()
	{
		try
		{
			QueryLobbiesOptions queryLobbiesOptions = new()
			{
				Filters = new List<QueryFilter>()
				{
					new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
				}
			};

			var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
			OnRefreshLobbies?.Invoke(queryResponse.Results);

		}
		catch (AuthenticationException e)
		{
			Debug.Log(e);
		}
	}
}
