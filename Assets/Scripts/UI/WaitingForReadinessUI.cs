using System;
using UnityEngine;

public class WaitingForReadinessUI : MonoBehaviour
{

	private void Start()
	{
		GameManager.Instance.OnLocalPlayerSetReady += OnLocalPlayerSetReady;
		GameManager.Instance.OnStateChanged += OnGameStateChanged;
		Hide();
	}

	private void OnDestroy()
	{
		GameManager.Instance.OnLocalPlayerSetReady -= OnLocalPlayerSetReady;
		GameManager.Instance.OnStateChanged -= OnGameStateChanged;
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void OnLocalPlayerSetReady()
	{
		Show();
	}

	private void OnGameStateChanged(object sender, EventArgs args)
	{
		Hide();
	}
}
