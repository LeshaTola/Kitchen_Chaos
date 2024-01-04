using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{

	[SerializeField] TextMeshProUGUI countdownText;

	const string POPUP_COUNTDOWN_ANIMATION_TRIGER = "PopUp";

	private Animator animator;
	private int prevCountdown;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
		Hide();
	}

	private void Instance_OnStateChanged(object sender, System.EventArgs e)
	{
		if (GameManager.Instance.IsCountdownTime())
		{
			Show();
		}
		else
		{
			Hide();
		}
	}
	private void Update()
	{
		int curentCountdown = Mathf.CeilToInt(GameManager.Instance.GetCountdownTimer());
		countdownText.text = curentCountdown.ToString();

		if (curentCountdown != prevCountdown)
		{
			prevCountdown = curentCountdown;
			animator.SetTrigger(POPUP_COUNTDOWN_ANIMATION_TRIGER);
			SounManager.Instance.PlayPopUpSound();
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