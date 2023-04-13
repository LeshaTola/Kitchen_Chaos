using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CuttingCounter;

public class ProgressBarUI : MonoBehaviour
{
	[SerializeField] private GameObject hasProgressBarGameObject;
	[SerializeField] private Image barImage;

	private IHasProgressBar hasProgressBar;
	private void Start() {
		hasProgressBar = hasProgressBarGameObject.GetComponent<IHasProgressBar>();
		if(hasProgressBar == null) {
			Debug.LogError("Object: " + hasProgressBarGameObject + "dont have IHasProgressBar");
		}
		hasProgressBar.OnProgressChanged += CuttingCounter_OnProgressChanged;
		Hide();
	}

	private void CuttingCounter_OnProgressChanged(object sender, IHasProgressBar.OnProgressChangedEventArgs e) {
		barImage.fillAmount = e.progressNormalised;

		Show();
		if (barImage.fillAmount <= 0 || barImage.fillAmount >= 1) {
			Hide();
		}
	}

	void Show() {
		gameObject.SetActive(true);
	}
	void Hide() {
		gameObject.SetActive(false);
	}

}
