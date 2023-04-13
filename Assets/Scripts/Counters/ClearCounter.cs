using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			// ���� �� ����� ������ ���
			if (player.HasKitchenObject()) {
				player.GetKitchenObject().SetKitchenObjectParent(this);
			}
		}
		else {
			// ���� �� ����� ���-�� ����
			if (!GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
				//���� ��� ���-�� �� �������
				if (player.HasKitchenObject()) {
					//� � ������ ���-�� ����
					if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
						// � ��� ���-�� ��� �������
						if(plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
							GetKitchenObject().DestroySelf();
					}
					else {
						// ���� � ������ �� �������
					}
				}
				else {
					// ���� � ������ ������ ���
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
			else {
				// ���� ��� �������
				if (player.HasKitchenObject()) {
					// � ������ ���-�� ����
					if(plateKitchenObject.TryToAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
						player.GetKitchenObject().DestroySelf();
				}
				else {
					// ������ �������
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
		}
	}
}
