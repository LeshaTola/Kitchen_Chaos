using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
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

	public async void CreateLobby(string lobbyName, bool isPrivate)
	{
		OnLobbyCreateStarted?.Invoke();
		try
		{
			JoinedLobby = await LobbyService.Instance.CreateLobbyAsync(
				lobbyName,
				KitchenGameMultiplayer.MAX_CLIENT_COUNT,
				new CreateLobbyOptions { IsPrivate = isPrivate });

			KitchenGameMultiplayer.Instance.StartHost();
			Loader.LoadNetwork(Loader.Scene.SelectCharacterScene);
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
			OnCreateLobbyFailed?.Invoke();
		}
	}

	public async void JoinLobbyByCode(string lobbyCode)
	{
		OnJoinLobbyStarted?.Invoke();
		try
		{
			JoinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
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
		if (JoinedLobby == null)
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
