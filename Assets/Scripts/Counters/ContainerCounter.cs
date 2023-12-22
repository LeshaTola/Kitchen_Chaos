using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public event EventHandler OnPlayerGrabedObject;

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject())
		{
			KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

			InteractServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractServerRpc()
	{
		InteractClientRpc();
	}

	[ClientRpc]
	private void InteractClientRpc()
	{
		OnPlayerGrabedObject?.Invoke(this, EventArgs.Empty);
	}
}
