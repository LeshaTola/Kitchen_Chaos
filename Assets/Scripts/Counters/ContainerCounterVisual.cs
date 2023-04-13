using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour {
	[SerializeField] private ContainerCounter containerCounter;
	private const string OPEN_CLOSE = "OpenClose";

	[SerializeField] private Animator animator;

	private void Awake() {
		animator = GetComponent<Animator>();
	}

	private void Start() {
		containerCounter.OnPlayerGrabedObject += ContainerCounter_OnPlayerGrabedObject;
	}

	private void ContainerCounter_OnPlayerGrabedObject(object sender, System.EventArgs e) {
		animator.SetTrigger(OPEN_CLOSE);
	}
}
