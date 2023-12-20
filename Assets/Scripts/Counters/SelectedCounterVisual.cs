using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
	[SerializeField] private BaseCounter basecounter;
	[SerializeField] private GameObject[] visualGameObject;

	void Start()
	{
		if (Player.LocalInstance != null)
		{
			Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		}
		else
		{
			Player.OnAnyPlayerSpawned += OnAnyPlayerSpawned;
		}
	}

	private void OnAnyPlayerSpawned(object sender, System.EventArgs e)
	{
		if (Player.LocalInstance != null)
		{
			Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
			Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		}
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
