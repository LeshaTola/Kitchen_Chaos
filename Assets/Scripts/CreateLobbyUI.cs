using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
	[SerializeField] private Button createButton;
	[SerializeField] private Button closeButton;
	[SerializeField] private TMP_InputField lobbyNameInputField;
	[SerializeField] private Toggle isPrivateToggle;
	[SerializeField] private LobbyUI lobbyUI;

	private void Awake()
	{
		createButton.onClick.AddListener(() =>
		{
			GameLobby.Instance.CreateLobby(lobbyNameInputField.text, isPrivateToggle.isOn);

		});

		closeButton.onClick.AddListener(() =>
		{
			lobbyUI.Show();
			Hide();
		});
	}

	private void Start()
	{
		Hide();
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
