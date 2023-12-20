using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{

	[SerializeField] private RecipeListSO recipeListSO;
	public event EventHandler OnRecipeSpawned;
	public event EventHandler OnRecipeDelivered;
	public event EventHandler OnDeliverySuccess;
	public event EventHandler OnDeliveryFail;


	public static DeliveryManager Instance { get; private set; }

	private List<RecipeSO> waitingRecipeSOList;
	private float RecipeTimer;
	private float RecipeTimerMax = 4f;
	private uint RecipeAmountMax = 4;
	private int deliverySuccessed = 0;

	private void Awake()
	{
		Instance = this;
		waitingRecipeSOList = new List<RecipeSO>();
	}

	private void Update()
	{
		if (GameManager.Instance.IsPlayingTime() && waitingRecipeSOList.Count < RecipeAmountMax)
		{
			RecipeTimer += Time.deltaTime;
			if (RecipeTimer >= RecipeTimerMax)
			{
				RecipeTimer = 0;
				int newRecipeSOId = UnityEngine.Random.Range(0, recipeListSO.RecipeSOList.Count);
				SpawnNewWaitingRecipeClientRpc(newRecipeSOId);
			}
		}
	}

	[ClientRpc]
	private void SpawnNewWaitingRecipeClientRpc(int newRecipeSOId)
	{
		RecipeSO recipeSO = recipeListSO.RecipeSOList[newRecipeSOId];
		waitingRecipeSOList.Add(recipeSO);
		OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
	}

	public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
	{
		for (int i = 0; i < waitingRecipeSOList.Count; i++)
		{
			RecipeSO waitingRecipe = waitingRecipeSOList[i];
			if (waitingRecipe.IngredientsList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
			{
				bool isRecipeCorrect = true;
				foreach (KitchenObjectSO kitchenObjectSO in waitingRecipe.IngredientsList)
				{
					bool isIngredientFound = false;
					foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
					{

						if (Equals(plateKitchenObjectSO.name, kitchenObjectSO.name))
						{
							isIngredientFound = true;
							break;
						}
					}
					if (!isIngredientFound)
					{
						isRecipeCorrect = false;
						break;
					}
				}

				if (isRecipeCorrect)
				{
					SuccessDeliveryServerRpc(i);
					return;
				}
			}
		}
		FailDeliveryServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void FailDeliveryServerRpc()
	{
		FailDeliveryClientRpc();
	}

	[ClientRpc]
	private void FailDeliveryClientRpc()
	{
		OnDeliveryFail.Invoke(this, EventArgs.Empty);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SuccessDeliveryServerRpc(int waitingRecipeSOId)
	{
		SuccessDeliveryClientRpc(waitingRecipeSOId);
	}

	[ClientRpc]
	private void SuccessDeliveryClientRpc(int waitingRecipeSOId)
	{
		waitingRecipeSOList.RemoveAt(waitingRecipeSOId);
		OnRecipeDelivered?.Invoke(this, EventArgs.Empty);
		OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
		deliverySuccessed++;
	}


	public List<RecipeSO> GetWaitingRecipeSOList()
	{
		return waitingRecipeSOList;
	}

	public int GetDeliverySuccessed()
	{
		return deliverySuccessed;
	}
}