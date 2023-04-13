using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoveFlashingBarUI : MonoBehaviour {

	[SerializeField] private StoveCounter stoveCounter;
	
	private Animator animator;
	private const string STOVE_FLASHING_PROGRESS_BAR = "IsWarning";

	private void Awake() {
		animator = GetComponent<Animator>();
	}

	private void Start () {
		stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
		animator.SetBool(STOVE_FLASHING_PROGRESS_BAR, false);
	}

	private void StoveCounter_OnProgressChanged(object sender, IHasProgressBar.OnProgressChangedEventArgs e) {
		float timeOffsetBeforeBern = 0.5f;
		bool isWarning = e.progressNormalised >= timeOffsetBeforeBern && stoveCounter.IsFried();

		if (isWarning) {
			animator.SetBool(STOVE_FLASHING_PROGRESS_BAR, true);
		}
		else {
			animator.SetBool(STOVE_FLASHING_PROGRESS_BAR, false);
		}
	}


}