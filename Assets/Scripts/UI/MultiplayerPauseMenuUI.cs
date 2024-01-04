using UnityEngine;

public class MultiplayerPauseMenuUI : MonoBehaviour
{

	private void Start()
	{
		GameManager.Instance.OnMultiplayerGamePaused += OnMultiplayerGamePaused;
		GameManager.Instance.OnMultiplayerGameUnPaused += OnMultiplayerGameUnPaused;

		Hide();
	}

	private void OnDestroy()
	{
		GameManager.Instance.OnMultiplayerGamePaused -= OnMultiplayerGamePaused;
		GameManager.Instance.OnMultiplayerGameUnPaused -= OnMultiplayerGameUnPaused;
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}

	private void OnMultiplayerGameUnPaused()
	{
		Hide();
	}

	private void OnMultiplayerGamePaused()
	{
		Show();
	}
}
