using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
	public static event EventHandler OnTrashed;

	new public static void ResetStaticData()
	{
		OnTrashed = null;
	}

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject())
		{
			KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
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
		OnTrashed?.Invoke(this, EventArgs.Empty);

	}
}
