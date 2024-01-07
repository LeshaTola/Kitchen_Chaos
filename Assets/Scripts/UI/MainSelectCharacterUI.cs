using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainSelectCharacterUI : MonoBehaviour
{
	[SerializeField] private Button readyButton;
	[SerializeField] private Button mainMenuButton;

	[SerializeField] private TextMeshProUGUI lobbyNameText;
	[SerializeField] private TextMeshProUGUI lobbyCodeText;

	private void Awake()
	{
		readyButton.onClick.AddListener(() =>
		{
			ReadinessController.Instance.SetReadiness();
		});

		mainMenuButton.onClick.AddListener(() =>
		{
			GameLobby.Instance.LeaveLobby();
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scene.MainMenu);
		});

		lobbyNameText.text = $"Lobby name: {GameLobby.Instance.JoinedLobby.Name}";
		lobbyCodeText.text = $"Lobby code: {GameLobby.Instance.JoinedLobby.LobbyCode}";
	}
}
