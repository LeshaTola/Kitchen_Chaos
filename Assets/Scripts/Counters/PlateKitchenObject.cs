using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

	[SerializeField] List<KitchenObjectSO> availableKitchenObjectList;
	private List<KitchenObjectSO> ingredientsList;

	public event EventHandler<OnAddIngredientEventArgs> OnAddIngredient;
	public class OnAddIngredientEventArgs
	{
		public KitchenObjectSO kitchenObjectSO;
	}

	protected override void Awake()
	{
		base.Awake();
		ingredientsList = new List<KitchenObjectSO>();
	}

	public bool TryToAddIngredient(KitchenObjectSO kitchenObjectSO)
	{
		if (!availableKitchenObjectList.Contains(kitchenObjectSO))
		{
			return false;
		}
		if (!ingredientsList.Contains(kitchenObjectSO))
		{
			ingredientsList.Add(kitchenObjectSO);
			OnAddIngredient?.Invoke(this, new OnAddIngredientEventArgs
			{
				kitchenObjectSO = kitchenObjectSO
			});
			return true;
		}
		else
		{
			return false;
		}
	}

	public List<KitchenObjectSO> GetKitchenObjectSOList()
	{
		return ingredientsList;
	}
}
