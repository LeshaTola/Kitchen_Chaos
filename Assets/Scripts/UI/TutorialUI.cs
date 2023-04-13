using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI keyMoveUpText;
	[SerializeField] private TextMeshProUGUI keyMoveDownText;
	[SerializeField] private TextMeshProUGUI keyMoveLeftText;
	[SerializeField] private TextMeshProUGUI keyMoveRightText;
	[SerializeField] private TextMeshProUGUI keyInteractText;
	[SerializeField] private TextMeshProUGUI keyInteractAltText;
	[SerializeField] private TextMeshProUGUI keyPauseText;

	private void Start() {
		GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
		GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
		UpdateVisual();
		Show();
	}

	private void GameManager_OnStateChanged(object sender, System.EventArgs e) {
		if (GameManager.Instance.IsCountdownTime()) {
			Hide();
		}
	}

	private void GameInput_OnBindingRebind(object sender, System.EventArgs e) {
		UpdateVisual();
	}

	public void UpdateVisual() {
		keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
		keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
		keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
		keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
		keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
		keyInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
		keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show() {
		gameObject.SetActive(true);
	}

}