using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectParent
{
	public void SetKitchenObject(KitchenObject kitchenObjectParent);
	public KitchenObject GetKitchenObject();
	public bool HasKitchenObject();
	public void ClearKitchenObject();
	public Transform GetKitchenObjectFollowTransform();

}
