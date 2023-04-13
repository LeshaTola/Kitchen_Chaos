using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour {

	float stepSoundTimer;
	float stepSoundTimerMax = 0.1f;
	Player player;

	private void Awake() {
		player = GetComponent<Player>();
	}

	private void Update() {
		stepSoundTimer += Time.deltaTime;
		if(stepSoundTimer > stepSoundTimerMax) {
			stepSoundTimer = 0;
			if (player.IsWalking()) {
				SounManager.Instance.PlayFootstepSound(player.transform.position);
			}
		}
	}
}