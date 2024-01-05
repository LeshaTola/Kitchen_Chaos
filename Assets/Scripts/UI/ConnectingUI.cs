using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

	private void Start()
	{
		KitchenGameMultiplayer.Instance.OnTryToConnecting += OnTryToConnecting;
		KitchenGameMultiplayer.Instance.OnFailToConnect += OnFailToConnect;

		Hide();
	}

	private void OnDestroy()
	{
		KitchenGameMultiplayer.Instance.OnTryToConnecting -= OnTryToConnecting;
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

	private void OnTryToConnecting()
	{
		Show();
	}

	private void OnFailToConnect()
	{
		Hide();
	}
}
