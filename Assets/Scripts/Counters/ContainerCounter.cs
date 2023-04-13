using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter {
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public event EventHandler OnPlayerGrabedObject;

	public override void Interact(Player player) {
		if (!player.HasKitchenObject()) {
			KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

			OnPlayerGrabedObject?.Invoke(this, EventArgs.Empty);
		}
	}
}
