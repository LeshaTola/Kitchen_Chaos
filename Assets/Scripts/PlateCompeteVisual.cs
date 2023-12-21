using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompeteVisual : MonoBehaviour
{
	[SerializeField] PlateKitchenObject plateKitchenObject;
	[Serializable]
	public struct KitchenObjectSO_GameObject
	{
		public KitchenObjectSO kitchenObjectSO;
		public GameObject gameObject;
	}
	[SerializeField] List<KitchenObjectSO_GameObject> ingridientsList;

	private void Start()
	{

		plateKitchenObject.OnAddIngredient += PlateKitchenObject_OnAddIngredient;

		foreach (KitchenObjectSO_GameObject ko in ingridientsList)
		{
			ko.gameObject.SetActive(false);
		}
	}

	private void PlateKitchenObject_OnAddIngredient(object sender, PlateKitchenObject.OnAddIngredientEventArgs e)
	{
		foreach (KitchenObjectSO_GameObject ko in ingridientsList)
		{
			if (ko.kitchenObjectSO == e.kitchenObjectSO)
			{
				ko.gameObject.SetActive(true);
			}
		}
	}
}
