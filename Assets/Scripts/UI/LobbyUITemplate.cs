using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUITemplate : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI lobbyNameText;

	private Lobby lobby;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			GameLobby.Instance.JoinLobbyById(lobby.Id);
		});
	}

	public void SetLobby(Lobby lobby)
	{
		this.lobby = lobby;
		UpdateUI();
	}

	private void UpdateUI()
	{
		lobbyNameText.text = lobby.Name;
	}
}
