using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {

	[SerializeField] private RecipeListSO recipeListSO;
	public event EventHandler OnRecipeSpawned;
	public event EventHandler OnRecipeDelivered;
	public event EventHandler OnDeliverySuccess;
	public event EventHandler OnDeliveryFail;


	public static DeliveryManager Instance { get; private set; }

	private List<RecipeSO> waitingRecipeSOList;
	private float RecipeTimer;
	private float RecipeTimerMax = 4f;
	private uint RecipeAmmountMax = 4;
	private int deliverySuccessed = 0;

	private void Awake() {
		Instance = this;
		waitingRecipeSOList = new List<RecipeSO>();
	}

	private void Update() {
		if (GameManager.Instance.IsPlayingTime() && waitingRecipeSOList.Count < RecipeAmmountMax) {
			RecipeTimer += Time.deltaTime;
			if (RecipeTimer >= RecipeTimerMax) {
				RecipeTimer = 0;
				RecipeSO recipeSO = recipeListSO.RecipeSOList[UnityEngine.Random.Range(0, recipeListSO.RecipeSOList.Count)];
				waitingRecipeSOList.Add(recipeSO);
				OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public void DeliveryRecipe(PlateKitchenObject plateKitchenObject) {
		for (int i = 0; i < waitingRecipeSOList.Count; i++) {
			RecipeSO waitingRecipe = waitingRecipeSOList[i];
			if (waitingRecipe.IngredientsList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
				bool isRecipeCorrect = true;
				foreach (KitchenObjectSO kitchenObjectSO in waitingRecipe.IngredientsList) {
					bool isIngredientFound = false;
					foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {

						if (Equals(plateKitchenObjectSO.name, kitchenObjectSO.name)) {
							isIngredientFound = true;
							break;
						}
					}
					if (!isIngredientFound) {
						isRecipeCorrect = false;
						break;
					}
				}
				if (isRecipeCorrect) {
					waitingRecipeSOList.RemoveAt(i);
					OnRecipeDelivered?.Invoke(this, EventArgs.Empty);
					OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
					deliverySuccessed++;
					return;
				}
			}
		}
		OnDeliveryFail.Invoke(this, EventArgs.Empty);
	}

	public List<RecipeSO> GetWaitingRecipeSOList() {
		return waitingRecipeSOList;
	}
	public int GetDeliverySuccessed() {
		return deliverySuccessed;
	}
}