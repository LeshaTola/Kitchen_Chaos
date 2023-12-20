using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
	[SerializeField] private Button HostButton;
	[SerializeField] private Button ClientButton;

	private void Awake()
	{
		HostButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.StartHost();
			Hide();
		});

		ClientButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.StartClient();
			Hide();
		});

	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
