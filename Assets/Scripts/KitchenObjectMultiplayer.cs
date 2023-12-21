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


		kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
		var kitchenObjectParentInterface = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

		kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(kitchenObjectParentInterface);
	}

	public KitchenObjectSO GetKitchenObject(int KitchenObjectIndex)
	{
		return kitchenObjectListSO.List[KitchenObjectIndex];
	}

	public int GetIndexOfKitchenObject(KitchenObjectSO kitchenObjectSO)
	{
		return kitchenObjectListSO.List.IndexOf(kitchenObjectSO);
	}
}
