using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
	public void SetKitchenObject(KitchenObject kitchenObjectParent);
	public KitchenObject GetKitchenObject();
	public bool HasKitchenObject();
	public void ClearKitchenObject();
	public Transform GetKitchenObjectFollowTransform();

	public NetworkObject GetNetworkObject();

}
