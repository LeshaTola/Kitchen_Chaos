using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SelectPlayerVisual : MonoBehaviour
{
	[SerializeField] private int clientIndex;
	[SerializeField] private GameObject readyGameObject;
	[SerializeField] private TextMeshPro playerNameText;
	[SerializeField] private Button kickButton;
	[SerializeField] private PlayerVisual playerVisual;

	private void Awake()
	{
		kickButton.onClick.AddListener(() =>
		{
			var clientData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(clientIndex);
			GameLobby.Instance.KickPlayer(clientData.id.ToString());
			KitchenGameMultiplayer.Instance.DisconnectClient(clientData.clientId);
		});
	}

	private void Start()
	{
		KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChanged;
		ReadinessController.Instance.OnPlayerSetReady += OnPlayerSetReady;

		kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

		UpdateVisual();
	}

	private void OnDestroy()
	{
		KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= OnPlayerDataNetworkListChanged;
	}

	private void OnPlayerSetReady()
	{
		UpdateVisual();
	}

	private void OnPlayerDataNetworkListChanged()
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(clientIndex))
		{
			Show();

			var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(clientIndex);
			readyGameObject.SetActive(ReadinessController.Instance.IsPlayerReady(playerData.clientId));

			playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetColor(playerData.colorIndex));

			playerNameText.text = playerData.name.ToString();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
