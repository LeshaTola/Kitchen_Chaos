using UnityEngine;
using UnityEngine.UI;

public class TestSetReadyUI : MonoBehaviour
{
	[SerializeField] private Button setReadyButton;

	private void Awake()
	{
		setReadyButton.onClick.AddListener(() =>
		{
			SelectCharacterReadinessController.Instance.SetReadiness();
			Hide();
		});
	}

	private void Hide()
	{
		gameObject.SetActive(false);

	}
}
