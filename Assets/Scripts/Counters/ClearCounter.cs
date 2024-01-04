using UnityEngine;

public class ClearCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			// Если на столе ничего нет
			if (player.HasKitchenObject())
			{
				player.GetKitchenObject().SetKitchenObjectParent(this);
			}
		}
		else
		{
			// Если на столе что-то есть
			if (!GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
			{
				//Если это что-то не тарелка
				if (player.HasKitchenObject())
				{
					//И у игрока что-то есть
					if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject))
					{
						// И это что-то это тарелка
						if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
							KitchenObject.DestroyKitchenObject(GetKitchenObject());
					}
				}
				else
				{
					// Если у игрока ничего нет
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
			else
			{
				// Если это тарелка
				if (player.HasKitchenObject())
				{
					// У игрока что-то есть
					if (plateKitchenObject.TryToAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
						KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
				}
				else
				{
					// Возьми тарелку
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
		}
	}
}
