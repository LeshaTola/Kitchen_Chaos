using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerResultUI : MonoBehaviour {

	[SerializeField] private Image background;
	[SerializeField] private TextMeshProUGUI deliveryResultText;
	[SerializeField] private Image deliveryResultImage;
	[Space]
	[SerializeField] private Sprite deliverySuccessedImage;
	[SerializeField] private Sprite deliveryFailedImage;
	[SerializeField] private Color deliverySuccessedColor;
	[SerializeField] private Color deliveryFailedColor;

	private const string POPUP = "PopUp";

	private Animator animator;

	private void Awake() {
		animator = GetComponent<Animator>();
	}

	private void Start() {
		DeliveryManager.Instance.OnDeliveryFail += DeliveryManager_OnDeliveryFail;
		DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
		gameObject.SetActive(false);
	}

	private void DeliveryManager_OnDeliverySuccess(object sender, System.EventArgs e) {
		gameObject.SetActive(true);
		background.color = deliverySuccessedColor;
		deliveryResultImage.sprite = deliverySuccessedImage;
		deliveryResultText.text = "Delivery\nSuccess";
		animator.SetTrigger(POPUP);
	}

	private void DeliveryManager_OnDeliveryFail(object sender, System.EventArgs e) {
		gameObject.SetActive(true);
		background.color = deliveryFailedColor;
		deliveryResultImage.sprite = deliveryFailedImage;
		deliveryResultText.text = "Delivery\nFail";
		animator.SetTrigger(POPUP);
	}
}