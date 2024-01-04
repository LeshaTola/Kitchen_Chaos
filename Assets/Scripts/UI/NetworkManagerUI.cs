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
			GameManager.Instance.StartHost();
			Hide();
		});

		ClientButton.onClick.AddListener(() =>
		{
			GameManager.Instance.StartClient();
			Hide();
		});

	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
