using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningUI : MonoBehaviour {

	[SerializeField] StoveCounter stoveCounter;


	private void Start() {
		stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
		Hide();
	}

	private void StoveCounter_OnProgressChanged(object sender, IHasProgressBar.OnProgressChangedEventArgs e) {
		float timeOffsetBeforeBern = 0.5f;
		bool isWarning = e.progressNormalised >= timeOffsetBeforeBern && stoveCounter.IsFried();

		if (isWarning) {
			Show();
		}
		else {
			Hide();
		}
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

}