using UnityEngine;

public class UI_Elements : MonoBehaviour
{

	[SerializeField] PlateKitchenObject plateKitchenObject;
	[SerializeField] Transform iconTemplatePrefab;

	private void Start()
	{
		plateKitchenObject.OnAddIngredient += PlateKitchenObject_OnAddIngredient;
	}

	private void PlateKitchenObject_OnAddIngredient(object sender, PlateKitchenObject.OnAddIngredientEventArgs e)
	{
		UpdateUI();
	}

	void UpdateUI()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		foreach (KitchenObjectSO element in plateKitchenObject.GetKitchenObjectSOList())
		{
			Transform icon = Instantiate(iconTemplatePrefab, transform);
			icon.GetComponent<IconTemplate>().SetIcon(element);
		}
	}
}
