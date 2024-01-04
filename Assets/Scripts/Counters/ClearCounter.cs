using UnityEngine;

public class ClearCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			// ���� �� ����� ������ ���
			if (player.HasKitchenObject())
			{
				player.GetKitchenObject().SetKitchenObjectParent(this);
			}
		}
		else
		{
			// ���� �� ����� ���-�� ����
			if (!GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
			{
				//���� ��� ���-�� �� �������
				if (player.HasKitchenObject())
				{
					//� � ������ ���-�� ����
					if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject))
					{
						// � ��� ���-�� ��� �������
						if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
							KitchenObject.DestroyKitchenObject(GetKitchenObject());
					}
				}
				else
				{
					// ���� � ������ ������ ���
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
			else
			{
				// ���� ��� �������
				if (player.HasKitchenObject())
				{
					// � ������ ���-�� ����
					if (plateKitchenObject.TryToAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
						KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
				}
				else
				{
					// ������ �������
					GetKitchenObject().SetKitchenObjectParent(player);
				}
			}
		}
	}
}
