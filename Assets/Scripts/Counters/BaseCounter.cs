using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {
	[SerializeField] private Transform spawnPoint;

	public static event EventHandler OnDoropSmth;

	public static void ResetStaticData() {
		OnDoropSmth = null;
	}

	private KitchenObject kitchenObject;

	public virtual void Interact(Player player) {
		//coming soon...
	}

	public virtual void InteractAlternative(Player player) { 

	}
	public void SetKitchenObject(KitchenObject kitchenObject) {
		this.kitchenObject = kitchenObject;
		OnDoropSmth?.Invoke(this, EventArgs.Empty);
	}

	public KitchenObject GetKitchenObject() {
		return kitchenObject;
	}

	public bool HasKitchenObject() {
		return kitchenObject != null;
	}

	public void ClearKitchenObject() {
		kitchenObject = null;
	}
	public Transform GetKitchenObjectFollowTransform() {
		return spawnPoint;
	}
}
