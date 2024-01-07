using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI messageText;
	[SerializeField] private Button closeButton;

	private void Awake()
	{
		closeButton.onClick.AddListener(Hide);
	}

	private void Start()
	{
		KitchenGameMultiplayer.Instance.OnFailToConnect += OnFailToConnect;
		GameLobby.Instance.OnLobbyCreateStarted += OnLobbyCreateStarted;
		GameLobby.Instance.OnCreateLobbyFailed += OnCreateLobbyFailed;

		GameLobby.Instance.OnJoinLobbyStarted += OnJoinLobbyStarted;
		GameLobby.Instance.OnJoinLobbyFailed += OnJoinLobbyFailed;
		GameLobby.Instance.OnQuickJoinLobbyFailed += OnQuickJoinLobbyFailed;

		Hide();
	}

	private void OnDestroy()
	{
		KitchenGameMultiplayer.Instance.OnFailToConnect -= OnFailToConnect;
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}

	private void OnQuickJoinLobbyFailed()
	{
		ShowMessage("Fail to quickJoin lobby!");
	}

	private void OnJoinLobbyFailed()
	{
		ShowMessage("Fail to join lobby!");
	}

	private void OnJoinLobbyStarted()
	{
		ShowMessage("Joining to lobby...");
	}

	private void OnCreateLobbyFailed()
	{
		ShowMessage("Fail to create lobby!");
	}

	private void OnLobbyCreateStarted()
	{
		ShowMessage("Lobby is creating");
	}

	private void OnFailToConnect()
	{
		if (string.IsNullOrEmpty(messageText.text))
		{
			ShowMessage("Fail to connect");
		}
		else
		{
			ShowMessage(NetworkManager.Singleton.DisconnectReason);
		}
	}

	private void ShowMessage(string message)
	{
		Show();
		messageText.text = message;
	}
}
