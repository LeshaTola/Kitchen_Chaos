using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public event EventHandler OnStateChanged;
	public event EventHandler OnGamePaused;
	public event EventHandler OnGameUnPaused;
	public static GameManager Instance { get; private set; }
	private enum State
	{
		WaitingForReadiness,
		Countdown,
		PlayingTime,
		GaveOver
	}


	[SerializeField] private float CountdownTimerMax = 3f;
	[SerializeField] private float PlayingTimeTimerMax = 10f;

	private float PlayingTimeTimer;
	private float CountdownTimer;
	private bool isGamePaused = false;

	State state;

	private void Awake()
	{
		Instance = this;
		state = State.WaitingForReadiness;
	}

	private void Start()
	{
		GameInput.Instance.Pause += GameInput_Pause;
		GameInput.Instance.InteractEvent += GameInput_InteractEvent;

		CountdownTimer = CountdownTimerMax;

		//THIS IS FOR DEBUGGING
		state = State.Countdown;
		OnStateChanged?.Invoke(this, EventArgs.Empty);
		//###############################
	}

	private void GameInput_InteractEvent(object sender, EventArgs e)
	{
		if (state == State.WaitingForReadiness)
		{
			state = State.Countdown;
			OnStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void GameInput_Pause(object sender, EventArgs e)
	{
		TogglePauseGame();
	}

	private void Update()
	{
		switch (state)
		{
			case State.WaitingForReadiness:
				break;
			case State.Countdown:
				OnStateChanged.Invoke(this, EventArgs.Empty);
				CountdownTimer -= Time.deltaTime;
				if (CountdownTimer <= 0)
				{
					CountdownTimer = CountdownTimerMax;
					state = State.PlayingTime;
				}
				break;
			case State.PlayingTime:
				OnStateChanged.Invoke(this, EventArgs.Empty);
				PlayingTimeTimer += Time.deltaTime;
				if (PlayingTimeTimer >= PlayingTimeTimerMax)
				{
					PlayingTimeTimer = 0;
					state = State.GaveOver;
				}
				break;
			case State.GaveOver:
				OnStateChanged.Invoke(this, EventArgs.Empty);
				break;
		}
	}

	public bool IsPlayingTime()
	{
		return state == State.PlayingTime;
	}
	public bool IsCountdownTime()
	{
		return state == State.Countdown;
	}
	public bool IsGameOver()
	{
		return state == State.GaveOver;
	}
	public float PlayingTimerNormolized()
	{
		return PlayingTimeTimer / PlayingTimeTimerMax;
	}
	public float GetCountdownTimer()
	{
		return CountdownTimer;
	}

	public void TogglePauseGame()
	{
		if (isGamePaused)
		{
			isGamePaused = false;
			Time.timeScale = 1.0f;
			OnGameUnPaused.Invoke(this, EventArgs.Empty);
		}
		else
		{
			isGamePaused = true;
			Time.timeScale = 0.0f;
			OnGamePaused.Invoke(this, EventArgs.Empty);
		}
	}
}