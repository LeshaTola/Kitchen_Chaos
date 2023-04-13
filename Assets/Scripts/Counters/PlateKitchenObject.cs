using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

	[SerializeField] List<KitchenObjectSO> availableKitchenObjectList;
	private List<KitchenObjectSO> ingredientsList;

	public event EventHandler<OnAddIngredientEventArds> OnAddIngredient;
	public class OnAddIngredientEventArds {
		public KitchenObjectSO kitchenObjectSO;
	}

	private void Awake() {
		ingredientsList= new List<KitchenObjectSO>();
	}
	public bool TryToAddIngredient(KitchenObjectSO kitchenObjectSO) {
		if(!availableKitchenObjectList.Contains(kitchenObjectSO)) {
			return false;
		}
		if (!ingredientsList.Contains(kitchenObjectSO)) {
			ingredientsList.Add(kitchenObjectSO);
			OnAddIngredient?.Invoke(this, new OnAddIngredientEventArds {
				kitchenObjectSO = kitchenObjectSO
			}) ;
			return true;
		}
		else {
			return false;
		}
	}

	public List<KitchenObjectSO> GetKitchenObjectSOList() { 
		return ingredientsList;
	}
}
