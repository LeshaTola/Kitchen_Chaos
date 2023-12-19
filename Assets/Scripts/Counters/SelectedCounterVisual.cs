using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
	[SerializeField] private BaseCounter basecounter;
	[SerializeField] private GameObject[] visualGameObject;

	void Start()
	{
		//Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
	}

	private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
	{
		if (e.baseCounter == basecounter)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	void Show()
	{
		foreach (GameObject go in visualGameObject)
		{

			go.SetActive(true);
		}
	}
	void Hide()
	{
		foreach (GameObject go in visualGameObject)
		{

			go.SetActive(false);
		}
	}
}
