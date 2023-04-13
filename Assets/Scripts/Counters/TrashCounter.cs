using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
	public static event EventHandler OnTrashed;

	new public static void ResetStaticData() {
		OnTrashed = null;
	}

	public override void Interact(Player player) {
		if (player.HasKitchenObject()) {
			OnTrashed?.Invoke(this,EventArgs.Empty);
			player.GetKitchenObject().DestroySelf();
		}
	}
}
