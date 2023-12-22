using Unity.Netcode;
using UnityEngine;

public class KitchenObjectMultiplayer : NetworkBehaviour
{
	[SerializeField] private KitchenObjectListSO kitchenObjectListSO;

	public static KitchenObjectMultiplayer Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
	{
		SpawnKitchenObjectServerRpc(GetIndexOfKitchenObject(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
		var kitchenObjectSO = GetKitchenObject(kitchenObjectSOIndex);
		Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefabs);

		var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
		kitchenObjectNetworkObject.Spawn();

		var kitchenObjectParentInterface = GetKitchenObjectParentFromNetworkObjectReference(kitchenObjectParentNetworkObjectReference);

		kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(kitchenObjectParentInterface);
	}

	public void DestroyKitchenObject(KitchenObject kitchenObject)
	{
		DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
	}

	[ServerRpc(RequireOwnership = false)]
	private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
		var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();


		kitchenObject.DestroySelf();
		ClearKitchenObjectForParentClientRpc(kitchenObjectNetworkObjectReference);
	}

	[ClientRpc]
	private void ClearKitchenObjectForParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
		var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

		kitchenObject.ClearKitchenObjectForParent();
	}


	public KitchenObjectSO GetKitchenObject(int KitchenObjectIndex)
	{
		return kitchenObjectListSO.List[KitchenObjectIndex];
	}

	public int GetIndexOfKitchenObject(KitchenObjectSO kitchenObjectSO)
	{
		return kitchenObjectListSO.List.IndexOf(kitchenObjectSO);
	}

	private IKitchenObjectParent GetKitchenObjectParentFromNetworkObjectReference(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
	{
		kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
		return kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
	}
}
