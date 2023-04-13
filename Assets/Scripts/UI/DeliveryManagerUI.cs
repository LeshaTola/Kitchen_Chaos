using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {

	[SerializeField] Transform conteiner;
	[SerializeField] Transform recepyIconTemplate;

	private void Start() {
		DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
		DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
		recepyIconTemplate.gameObject.SetActive(false);
		UpdateUI();
	}

	private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e) {
		UpdateUI();
	}

	private void DeliveryManager_OnRecipeDelivered(object sender, System.EventArgs e) {
		UpdateUI();
	}

	void UpdateUI() {
		foreach (Transform child in transform) {
			if (child == recepyIconTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
			Transform recepyIcon = Instantiate(recepyIconTemplate, conteiner);
			recepyIcon.gameObject.SetActive(true);
			recepyIcon.GetComponent<RecipeIconTemplate>().SetRecipeName(recipeSO);
			recepyIcon.GetComponent<RecipeIconTemplate>().UpdateUI(recipeSO);
		}
	}
}