using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour {
	[SerializeField] StoveCounter stoveCounter;

	private AudioSource audioSource;
	private bool isWarning = false;
	private float warningPlaySoundTimer = 0.2f;
	private void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	private void Start() {
		stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
		stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
	}

	private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
		bool isFrying = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
		if (isFrying) {
			audioSource.Play();
		}
		else {
			audioSource.Stop();
		}
	}

	private void StoveCounter_OnProgressChanged(object sender, IHasProgressBar.OnProgressChangedEventArgs e) {
		float timeOffsetBeforeBern = 0.5f;
		isWarning = e.progressNormalised >= timeOffsetBeforeBern && stoveCounter.IsFried();
	}

	private void Update() {
		if (isWarning) {
			warningPlaySoundTimer -= Time.deltaTime;
			if (warningPlaySoundTimer <= 0) {
				float warningPlaySoundTimerMax = 0.2f;
				warningPlaySoundTimer = warningPlaySoundTimerMax;
				SounManager.Instance.PlayWarningSound(stoveCounter.transform.position);
			}
		}
	}
}
