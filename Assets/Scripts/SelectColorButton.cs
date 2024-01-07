using UnityEngine;
using UnityEngine.UI;

public class SelectColorButton : MonoBehaviour
{
	[SerializeField] private int colorIndex;
	[SerializeField] private Image image;
	[SerializeField] private GameObject selectedGameObject;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			KitchenGameMultiplayer.Instance.ChangePlayerColor(colorIndex);
		});
	}

	private void Start()
	{
		KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChanged;
		image.color = KitchenGameMultiplayer.Instance.GetColor(colorIndex);
		UpdateIsSelected();
	}

	private void OnDestroy()
	{
		KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= OnPlayerDataNetworkListChanged;
	}

	private void OnPlayerDataNetworkListChanged()
	{
		UpdateIsSelected();
	}

	private void UpdateIsSelected()
	{
		var playerData = KitchenGameMultiplayer.Instance.GetPlayerData();

		if (playerData.colorIndex == colorIndex)
		{
			selectedGameObject.SetActive(true);
		}
		else
		{
			selectedGameObject.SetActive(false);
		}
	}
}
