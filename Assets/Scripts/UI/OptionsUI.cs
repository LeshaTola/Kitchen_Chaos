using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {
	[SerializeField] private Button soundEffectButton;
	[SerializeField] private GameObject rebindKeyUI;

	[Header("Кнопки настроек")]
	[SerializeField] private Button musicButton;
	[SerializeField] private Button closeButton;

	[SerializeField] private TextMeshProUGUI soundText;
	[SerializeField] private TextMeshProUGUI musicText;

	[Space(20)]
	[Header("Кнопки управления")]
	[SerializeField] private Button moveUpButton;
	[SerializeField] private Button moveDownButton;
	[SerializeField] private Button moveRightButton;
	[SerializeField] private Button moveLeftButton;
	[SerializeField] private Button interactButton;
	[SerializeField] private Button interactAltButton;
	[SerializeField] private Button pauseButton;

	[SerializeField] private TextMeshProUGUI moveUpText;
	[SerializeField] private TextMeshProUGUI moveDownText;
	[SerializeField] private TextMeshProUGUI moveRightText;
	[SerializeField] private TextMeshProUGUI moveLeftText;
	[SerializeField] private TextMeshProUGUI interactText;
	[SerializeField] private TextMeshProUGUI interactAltText;
	[SerializeField] private TextMeshProUGUI pauseText;

	public static OptionsUI Instance { get; private set; }

	private void Awake() {
		Instance = this;

		soundEffectButton.onClick.AddListener(() => {
			SounManager.Instance.ChangeVolume();
			UpdateVisual();
		});
		musicButton.onClick.AddListener(() => {
			MusicManager.Instance.ChangeVolume();
			UpdateVisual();
		});
		closeButton.onClick.AddListener(() => {
			Hide();
		});

		moveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveUp); });
		moveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveDown); });
		moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveLeft); });
		moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveRight); });
		interactButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
		interactAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlt); });
		pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
	}

	private void Start() {
		GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
		Hide();
		UpdateVisual();
	}

	private void GameManager_OnGameUnpaused(object sender, System.EventArgs e) {
		Hide();
	}

	private void UpdateVisual() {
		soundText.text = "Sound effects: " + Mathf.Round(SounManager.Instance.GetVolume() * 10);
		musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10);

		moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
		moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
		moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
		moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
		interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
		interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
		pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show() {
		gameObject.SetActive(true);
	}

	public void HideRebindKeyUI() {
		rebindKeyUI.SetActive(false);
	}

	public void ShowRebindKeyUI() {
		rebindKeyUI.SetActive(true);
	}

	private void RebindBinding(GameInput.Binding binding) {
		ShowRebindKeyUI();
		GameInput.Instance.RebindBinding(binding, () => {
			HideRebindKeyUI();
			UpdateVisual();
		});
	}
}
