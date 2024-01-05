using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostIsDisconnectedUI : MonoBehaviour
{
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
		NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
		Hide();
	}

	void Show()
	{
		gameObject.SetActive(true);
	}

	void Hide()
	{
		gameObject.SetActive(false);
	}

	private void OnClientDisconnectCallback(ulong clientId)
	{
		if (clientId == NetworkManager.ServerClientId)
		{
			Show();
		}
	}
}