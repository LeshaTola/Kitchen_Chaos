using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CuttingCounter : BaseCounter,IHasProgressBar {

	public event EventHandler<IHasProgressBar.OnProgressChangedEventArgs> OnProgressChanged;
	public event EventHandler OnCut;
	public static event EventHandler OnAnyCut;

	new public static void ResetStaticData() {
		OnAnyCut = null;
	}

	[SerializeField] private CuttingRecepySO[] cuttingRecepySOArray;
	
	private int cuttingProgress;
	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			// Если на столе ничего нет
			if (player.HasKitchenObject()) {
				// У игрока что-то есть
				if (HasOutputWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					// Ингридиенты подходят по рецепту
					player.GetKitchenObject().SetKitchenObjectParent(this);
					cuttingProgress = 0;
					CuttingRecepySO cuttingRecepySO = GetCuttingRecepySO(GetKitchenObject().GetKitchenObjectSO());
					OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
						progressNormalised = (float)cuttingProgress / cuttingRecepySO.cuttingProgressMax
					});

				}

			}
		}
		else {
			// на столе что-то есть
			if (player.HasKitchenObject()) {
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					// И это что-то это тарелка
					if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
						GetKitchenObject().DestroySelf();
				}
				else {
					// Если у игрока не тарелка
				}
			}
			else {
				GetKitchenObject().SetKitchenObjectParent(player);
				OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
					progressNormalised = 0
				});
			}

		}
	}

	public override void InteractAlternative(Player player) {
		if(HasKitchenObject() && HasOutputWithInput(GetKitchenObject().GetKitchenObjectSO())) {
			KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
			cuttingProgress++;
			CuttingRecepySO cuttingRecepySO = GetCuttingRecepySO(GetKitchenObject().GetKitchenObjectSO());

			OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
				progressNormalised = (float) cuttingProgress / cuttingRecepySO.cuttingProgressMax
			}) ;
			OnCut?.Invoke(this, EventArgs.Empty);
			OnAnyCut?.Invoke(this, EventArgs.Empty);

			if (cuttingProgress >= GetCuttingRecepySO(GetKitchenObject().GetKitchenObjectSO()).cuttingProgressMax) {
				GetKitchenObject().DestroySelf();
				KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
			}
		}
	}

	private bool HasOutputWithInput(KitchenObjectSO input) {
		CuttingRecepySO cuttingRecepySO = GetCuttingRecepySO(input);

		if (cuttingRecepySO != null) {
			return true;
		}
		return false;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO input) {
		CuttingRecepySO cuttingRecepySO = GetCuttingRecepySO(input);

		if (cuttingRecepySO != null) {
			return cuttingRecepySO.output;
		}
		return null;
	}

	private CuttingRecepySO GetCuttingRecepySO(KitchenObjectSO input) {
		foreach (CuttingRecepySO cuttingRecepySO in cuttingRecepySOArray) {
			if (input == cuttingRecepySO.input)
				return cuttingRecepySO;
		}
		return null;
	}
}
