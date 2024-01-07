using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
	[SerializeField] private Button mainMenuButton;

	[SerializeField] private CreateLobbyUI createLobbyUI;
	[SerializeField] private Button createLobbyButton;
	[SerializeField] private Button quickJoinButton;

	[SerializeField] private Button joinByCodeButton;
	[SerializeField] private TMP_InputField codeInputField;


	[SerializeField] private TMP_InputField playerNameInputField;

	[SerializeField] private LobbyUITemplate lobbyUITemplate;
	[SerializeField] private Transform lobbyUIContainer;

	private void Awake()
	{
		mainMenuButton.onClick.AddListener(() =>
		{
			GameLobby.Instance.LeaveLobby();
			Loader.Load(Loader.Scene.MainMenu);
		});

		quickJoinButton.onClick.AddListener(() =>
		{
			GameLobby.Instance.QuickJoin();
		});

		joinByCodeButton.onClick.AddListener(() =>
		{
			GameLobby.Instance.JoinLobbyByCode(codeInputField.text);
		});

		createLobbyButton.onClick.AddListener(() =>
		{
			createLobbyUI.Show();
		});

		playerNameInputField.text = KitchenGameMultiplayer.Instance.PlayerName;
		playerNameInputField.onValueChanged.AddListener((string name) =>
		{
			KitchenGameMultiplayer.Instance.PlayerName = name;
		});
	}

	private void Start()
	{
		UpdateLobbyList(new List<Lobby>());
		GameLobby.Instance.OnRefreshLobbies += OnRefreshLobbies;
	}

	private void OnDestroy()
	{
		GameLobby.Instance.OnRefreshLobbies -= OnRefreshLobbies;
	}

	private void OnRefreshLobbies(List<Lobby> lobbies)
	{
		UpdateLobbyList(lobbies);
	}

	private void UpdateLobbyList(List<Lobby> lobbies)
	{
		while (lobbyUIContainer.childCount > 0)
		{
			Destroy(lobbyUIContainer.GetChild(0));
		}

		foreach (var lobby in lobbies)
		{
			var newLobbyUI = Instantiate(lobbyUITemplate, lobbyUIContainer);
			newLobbyUI.SetLobby(lobby);
		}
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
