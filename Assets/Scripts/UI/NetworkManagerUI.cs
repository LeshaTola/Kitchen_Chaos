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
			KitchenGameMultiplayer.Instance.StartHost();
			Loader.LoadNetwork(Loader.Scene.SelectCharacterScene);
		});

		ClientButton.onClick.AddListener(() =>
		{
			KitchenGameMultiplayer.Instance.StartClient();
		});

	}
}
