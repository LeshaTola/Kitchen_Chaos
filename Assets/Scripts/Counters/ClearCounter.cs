using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			// Если на столе ничего нет
			if (player.HasKitchenObject()) {
				player.GetKitchenObject().SetKitchenObjectParent(this);
			}
		}
		else {
			// Если на столе что-то есть
			if (!GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
				//Если это что-то не тарелка
				if (player.HasKitchenObject()) {
					//И у игрока что-то есть
					if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
						// И это что-то это тарелка
						if(plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
							GetKitchenObject().DestroySelf();
					}
					else {
						// Если у игрока не тарелка
					}
				}
				else {
					// Если у игрока ничего нет
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
			else {
				// Если это тарелка
				if (player.HasKitchenObject()) {
					// У игрока что-то есть
					if(plateKitchenObject.TryToAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
						player.GetKitchenObject().DestroySelf();
				}
				else {
					// Возьми тарелку
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
		}
	}
}
