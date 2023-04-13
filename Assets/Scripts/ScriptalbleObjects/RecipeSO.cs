using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {
	public string recepyName;
	public List<KitchenObjectSO> IngredientsList;
}