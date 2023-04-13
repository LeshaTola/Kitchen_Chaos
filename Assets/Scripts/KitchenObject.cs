using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour {
	[SerializeField] KitchenObjectSO kitchenObjectSO;


	private IKitchenObjectParent kitchenObjectParent;

	public KitchenObjectSO GetKitchenObjectSO() { return kitchenObjectSO; }
	public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
		if (this.kitchenObjectParent != null) {
			this.kitchenObjectParent.ClearKitchenObject();
		}

		this.kitchenObjectParent = kitchenObjectParent;

		if (kitchenObjectParent.HasKitchenObject()) {
			Debug.LogError("Kitchen object Parent olredy exist");
		}
		kitchenObjectParent.SetKitchenObject(this);

		transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
		transform.localPosition = Vector3.zero;
	}

	public IKitchenObjectParent GetKitchenObjectParent() {
		return this.kitchenObjectParent;
	}

	public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
		Transform kitchenObjectPrefapTransform = Instantiate(kitchenObjectSO.prefabs);
		kitchenObjectPrefapTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(kitchenObjectParent);
	}

	public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
		if(this is PlateKitchenObject) {
			plateKitchenObject = this as PlateKitchenObject;
			return true;
		}
		else {
			plateKitchenObject = null;
			return false;
		}
	}

	public void DestroySelf() {
		GetKitchenObjectParent().SetKitchenObject(null);
		Destroy(gameObject);
	}
}