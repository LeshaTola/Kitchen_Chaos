using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

	public event EventHandler OnStateChanged;
	public event EventHandler OnLocalGamePaused;
	public event EventHandler OnLocalGameUnPaused;
	public event Action OnMultiplayerGamePaused;
	public event Action OnMultiplayerGameUnPaused;

	public event Action OnLocalPlayerSetReady;

	private Dictionary<ulong, bool> playerReadyDictionary;
	private Dictionary<ulong, bool> playerPauseDictionary;

	public static GameManager Instance { get; private set; }

	private enum State
	{
		WaitingForReadiness,
		Countdown,
		PlayingTime,
		GaveOver
	}


	[SerializeField] private float countdownTimerMax = 3f;
	[SerializeField] private float playingTimeTimerMax = 10f;

	private NetworkVariable<bool> multiplayerGamePause = new NetworkVariable<bool>(false);
	private NetworkVariable<float> playingTimeTimer = new NetworkVariable<float>(3f);
	private NetworkVariable<float> countdownTimer = new NetworkVariable<float>(0f);
	private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingForReadiness);

	private bool isLocalGamePaused = false;
	private bool isLocalPlayerReady = false;
	private bool needToTestPause = false;

	private void Awake()
	{
		Instance = this;
	}

	public override void OnNetworkSpawn()
	{
		state.OnValueChanged += OnStateValueChanged;
		multiplayerGamePause.OnValueChanged += OnMultiplayerGamePauseValueChanged;
		NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

		playerReadyDictionary = new Dictionary<ulong, bool>();
		playerPauseDictionary = new Dictionary<ulong, bool>();

		countdownTimer.Value = countdownTimerMax;
		playingTimeTimer.Value = 0;
	}


	private void Start()
	{
		GameInput.Instance.Pause += GameInput_Pause;
		GameInput.Instance.InteractEvent += GameInput_InteractEvent;

	}

	private void Update()
	{
		if (!IsServer)
		{
			return;
		}

		switch (state.Value)
		{
			case State.WaitingForReadiness:
				break;
			case State.Countdown:
				countdownTimer.Value -= Time.deltaTime;

				if (countdownTimer.Value <= 0)
				{
					countdownTimer.Value = countdownTimerMax;
					state.Value = State.PlayingTime;
				}
				break;
			case State.PlayingTime:
				playingTimeTimer.Value += Time.deltaTime;

				if (playingTimeTimer.Value >= playingTimeTimerMax)
				{
					playingTimeTimer.Value = 0;
					state.Value = State.GaveOver;
				}
				break;
			case State.GaveOver:
				break;
		}
	}

	private void LateUpdate()
	{
		if (needToTestPause)
		{
			needToTestPause = false;
			TestMultiplayerPause();
		}
	}

	public void StartHost()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCollback;
		NetworkManager.Singleton.StartHost();
	}

	public void StartClient()
	{
		NetworkManager.Singleton.StartClient();
	}

	private void GameInput_InteractEvent(object sender, EventArgs e)
	{
		if (state.Value == State.WaitingForReadiness)
		{
			isLocalPlayerReady = true;
			OnLocalPlayerSetReady?.Invoke();

			SetReadinessServerRpc();
		}
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
			state.Value = State.Countdown;
		}
	}

	private void GameInput_Pause(object sender, EventArgs e)
	{
		TogglePauseGame();
	}

	public bool IsPlayingTime()
	{
		return state.Value == State.PlayingTime;
	}

	public bool IsCountdownTime()
	{
		return state.Value == State.Countdown;
	}

	public bool IsGameOver()
	{
		return state.Value == State.GaveOver;
	}

	public float PlayingTimerNormalized()
	{
		return playingTimeTimer.Value / playingTimeTimerMax;
	}

	public float GetCountdownTimer()
	{
		return countdownTimer.Value;
	}

	public void TogglePauseGame()
	{
		if (isLocalGamePaused)
		{
			isLocalGamePaused = false;
			OnLocalGameUnPaused.Invoke(this, EventArgs.Empty);

			UnPauseMultiplayerGameServerRpc();
		}
		else
		{
			isLocalGamePaused = true;
			OnLocalGamePaused.Invoke(this, EventArgs.Empty);

			PauseMultiplayerGameServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void PauseMultiplayerGameServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
		TestMultiplayerPause();
	}

	[ServerRpc(RequireOwnership = false)]
	private void UnPauseMultiplayerGameServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
		TestMultiplayerPause();

	}

	private void TestMultiplayerPause()
	{
		foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
			{
				multiplayerGamePause.Value = true;
				return;
			}
		}

		multiplayerGamePause.Value = false;
	}

	private void OnStateValueChanged(State PrevValue, State NewValue)
	{
		OnStateChanged?.Invoke(this, EventArgs.Empty);
	}

	private void OnMultiplayerGamePauseValueChanged(bool previousValue, bool newValue)
	{
		if (newValue == true)
		{
			Time.timeScale = 0.0f;
			OnMultiplayerGamePaused?.Invoke();
		}
		else
		{
			Time.timeScale = 1.0f;
			OnMultiplayerGameUnPaused?.Invoke();
		}
	}


	private void OnClientDisconnectCallback(ulong clientId)
	{
		needToTestPause = true;
	}

	private void ConnectionApprovalCollback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		if (state.Value == State.WaitingForReadiness)
		{
			response.Approved = true;
			response.CreatePlayerObject = true;
		}
		else
		{
			response.Approved = false;
		}
	}

}