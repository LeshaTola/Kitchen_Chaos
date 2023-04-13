using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeIconTemplate : MonoBehaviour {

	[SerializeField] TextMeshProUGUI recepyText;
	[SerializeField] Transform conteiner;
	[SerializeField] Transform IngredientImageTemplate;

	public void SetRecipeName(RecipeSO recipeSo) {
		recepyText.text = recipeSo.name;
	}

	private void Start() {
		IngredientImageTemplate.gameObject.SetActive(false);
	}
	public void UpdateUI(RecipeSO recipeSo) {
		foreach (Transform child in conteiner) {
			if (child == IngredientImageTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (KitchenObjectSO kitchenObjectSO in recipeSo.IngredientsList) {
			Transform ingredientIcon = Instantiate(IngredientImageTemplate, conteiner);
			ingredientIcon.gameObject.SetActive(true);
			ingredientIcon.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
		}
	}
}