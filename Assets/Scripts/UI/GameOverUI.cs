using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI countOfDeliveryText;

	private void Start()
	{
		GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
		Hide();
	}

	private void Instance_OnStateChanged(object sender, System.EventArgs e)
	{
		if (GameManager.Instance.IsGameOver())
		{
			Show();
			countOfDeliveryText.text = DeliveryManager.Instance.GetDeliverySuccessed().ToString();
		}
		else
		{
			Hide();
		}
	}

	void Show()
	{
		gameObject.SetActive(true);
	}

	void Hide()
	{
		gameObject.SetActive(false);
	}
}
