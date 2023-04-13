using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingTimerUI : MonoBehaviour
{
	[SerializeField] Image ClockImage;

	private void Start() {
		GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
	}

	private void Instance_OnStateChanged(object sender, System.EventArgs e) {
		if (GameManager.Instance.IsPlayingTime()) {
			Show();
		}
		else {
			Hide();
		}
	}
	private void Update() {
		ClockImage.fillAmount = GameManager.Instance.PlayingTimerNormolized();
	}

	void Show() {
		gameObject.SetActive(true);
	}

	void Hide() {
		gameObject.SetActive(false);
	}
}
