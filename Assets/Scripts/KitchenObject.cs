using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
	[SerializeField] KitchenObjectSO kitchenObjectSO;

	private IKitchenObjectParent kitchenObjectParent;
	private FollowObject followObject;

	protected virtual void Awake()
	{
		followObject = GetComponent<FollowObject>();
	}

	public KitchenObjectSO GetKitchenObjectSO() { return kitchenObjectSO; }

	public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
	{
		SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
		SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
	}

	[ClientRpc]
	private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
		kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
		var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

		if (this.kitchenObjectParent != null)
		{
			this.kitchenObjectParent.ClearKitchenObject();
		}

		this.kitchenObjectParent = kitchenObjectParent;

		if (kitchenObjectParent.HasKitchenObject())
		{
			Debug.LogError("Kitchen object Parent already exist");
		}
		kitchenObjectParent.SetKitchenObject(this);

		followObject.SetFollowTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
	}

	public IKitchenObjectParent GetKitchenObjectParent()
	{
		return kitchenObjectParent;
	}

	public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
	{
		KitchenObjectMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
	}

	public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
	{
		if (this is PlateKitchenObject)
		{
			plateKitchenObject = this as PlateKitchenObject;
			return true;
		}
		else
		{
			plateKitchenObject = null;
			return false;
		}
	}

	public void DestroySelf()
	{
		GetKitchenObjectParent().SetKitchenObject(null);
		Destroy(gameObject);
	}
}