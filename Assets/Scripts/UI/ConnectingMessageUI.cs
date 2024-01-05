using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingMessageUI : MonoBehaviour
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
	private void OnFailToConnect()
	{
		Show();

		messageText.text = NetworkManager.Singleton.DisconnectReason;

		if (string.IsNullOrEmpty(messageText.text))
		{
			messageText.text = "Fail to connect";
		}
	}
}
