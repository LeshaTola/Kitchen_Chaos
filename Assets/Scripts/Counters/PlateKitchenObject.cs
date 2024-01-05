using System;
using System.Collections.Generic;
using Unity.Netcode;
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
			AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetIndexOfKitchenObject(kitchenObjectSO));
			return true;
		}
		else
		{
			return false;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void AddIngredientServerRpc(int kitchenObjectSOIndex)
	{
		AddIngredientClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void AddIngredientClientRpc(int kitchenObjectSOIndex)
	{
		var kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObject(kitchenObjectSOIndex);
		ingredientsList.Add(kitchenObjectSO);
		OnAddIngredient?.Invoke(this, new OnAddIngredientEventArgs
		{
			kitchenObjectSO = kitchenObjectSO
		});
	}

	public List<KitchenObjectSO> GetKitchenObjectSOList()
	{
		return ingredientsList;
	}
}
