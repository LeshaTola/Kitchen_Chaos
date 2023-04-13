using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCallBack : MonoBehaviour {
	private bool firstUpdate = true;

	private void Update() {
		if (firstUpdate) {
			firstUpdate= false;
			Loader.LoadCallBackScene();
		}
	}
}