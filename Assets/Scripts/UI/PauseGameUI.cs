using UnityEngine;
using UnityEngine.UI;

public class PauseGameUI : MonoBehaviour
{

	[SerializeField] private Button resumeButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button optionsButton;

	private void Awake()
	{
		resumeButton.onClick.AddListener(() =>
		{
			GameManager.Instance.TogglePauseGame();
		});
		mainMenuButton.onClick.AddListener(() =>
		{
			Loader.Load(Loader.Scene.MainMenu);
		});
		optionsButton.onClick.AddListener(() =>
		{
			OptionsUI.Instance.Show();
		});
	}

	private void Start()
	{
		GameManager.Instance.OnLocalGamePaused += OnLocalGamePaused;
		GameManager.Instance.OnLocalGameUnPaused += OnLocalGameUnpaused;

		Hide();
	}

	private void OnDestroy()
	{
		GameManager.Instance.OnLocalGamePaused -= OnLocalGamePaused;
		GameManager.Instance.OnLocalGameUnPaused -= OnLocalGameUnpaused;
	}

	private void OnLocalGameUnpaused(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void OnLocalGamePaused(object sender, System.EventArgs e)
	{
		Show();
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);

	}

}
