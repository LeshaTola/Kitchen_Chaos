using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{

	[SerializeField] Transform container;
	[SerializeField] Transform recipeIconTemplate;

	private void Start()
	{
		DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
		DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
		recipeIconTemplate.gameObject.SetActive(false);
		UpdateUI();
	}

	private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e)
	{
		UpdateUI();
	}

	private void DeliveryManager_OnRecipeDelivered(object sender, System.EventArgs e)
	{
		UpdateUI();
	}

	void UpdateUI()
	{
		foreach (Transform child in transform)
		{
			if (child == recipeIconTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
		{
			Transform recipeIcon = Instantiate(recipeIconTemplate, container);
			recipeIcon.gameObject.SetActive(true);
			recipeIcon.GetComponent<RecipeIconTemplate>().SetRecipeName(recipeSO);
			recipeIcon.GetComponent<RecipeIconTemplate>().UpdateUI(recipeSO);
		}
	}
}