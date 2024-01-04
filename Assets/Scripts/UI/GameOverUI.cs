using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI countOfDeliveryText;
	[SerializeField] Button goToMainMenuButton;

	private void Awake()
	{
		goToMainMenuButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scene.MainMenu);
		});
	}

	private void Start()
	{
		GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
		Hide();
	}

	private void Instance_OnStateChanged(object sender, System.EventArgs e)
	{
		if (GameManager.Instance.IsGameOver())
		{
			Show();
			countOfDeliveryText.text = DeliveryManager.Instance.GetDeliverySuccessed().ToString();
		}
		else
		{
			Hide();
		}
	}

	void Show()
	{
		gameObject.SetActive(true);
	}

	void Hide()
	{
		gameObject.SetActive(false);
	}
}
