using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgressBar {

	public event EventHandler<IHasProgressBar.OnProgressChangedEventArgs> OnProgressChanged;
	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
	public class OnStateChangedEventArgs {
		public State state;
	}

	public enum State {
		Idle,
		Frying,
		Fried,
		Burned
	}

	private State state;

	[SerializeField] private FryingRecepySO[] fryingRecepySOArray;
	[SerializeField] private BurningRecepySO[] burningRecepySOArray;

	private float fryingTimer;
	private float burningTimer;
	private FryingRecepySO fryingRecepySO;
	private BurningRecepySO burningRecepySO;

	private void Start() {
		state = State.Idle;
	}

	private void Update() {
		if (HasKitchenObject()) {
			switch (state) {
				case State.Idle:
					break;
				case State.Frying:
					fryingTimer += Time.deltaTime;

					OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
						progressNormalised = fryingTimer / fryingRecepySO.fryingTimerMax
					});

					if (fryingTimer >= fryingRecepySO.fryingTimerMax) {
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(GetOutputForInput(fryingRecepySO.input), this);
						burningRecepySO = GetBurningRecepySO(GetKitchenObject().GetKitchenObjectSO());
						fryingTimer = 0;
						state = State.Fried;
						OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
							state = state
						});
					}
					break;
				case State.Fried:
					burningTimer += Time.deltaTime;


					OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
						progressNormalised = burningTimer / burningRecepySO.burningTimerMax
					});

					if (burningTimer >= burningRecepySO.burningTimerMax) {
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(burningRecepySO.output, this);

						burningTimer = 0;
						state = State.Burned;
						OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
							state = state
						});
					}
					break;
				case State.Burned:
					OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
						progressNormalised = 0
					});
					break;
			}
		}
	}

	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			// На стойке Ничего нет
			if (player.HasKitchenObject()) {
				// У игрока что-то есть
				if (HasOutputWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					// Ингридиенты подходят
					player.GetKitchenObject().SetKitchenObjectParent(this);
					fryingRecepySO = GetFryingRecepySO(GetKitchenObject().GetKitchenObjectSO());
					OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
						progressNormalised = 0
					});
					state = State.Frying;
					burningTimer = 0;
					fryingTimer = 0;
					OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
						state = state
					});
				}
			}
		}
		else {
			// На стойке Что-то есть
			if (player.HasKitchenObject()) {
				// У игрока Что-то есть
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					// И это что-то это тарелка
					if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
						GetKitchenObject().DestroySelf();

					state = State.Idle;
					burningTimer = 0;
					fryingTimer = 0;
					OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
						state = state
					});
					OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
						progressNormalised = 0
					});
				}
				else {
					// Если у игрока не тарелка
				}
			}
			else {
				// У игрока ничего нет
				GetKitchenObject().SetKitchenObjectParent(player);
				state = State.Idle;
				burningTimer = 0;
				fryingTimer = 0;
				OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
					state = state
				});
				OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs() {
					progressNormalised = 0
				});
			}

		}
	}
	private bool HasOutputWithInput(KitchenObjectSO input) {
		FryingRecepySO fryingRecepySO = GetFryingRecepySO(input);

		if (fryingRecepySO != null) {
			return true;
		}
		return false;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO input) {
		FryingRecepySO fryingRecepySO = GetFryingRecepySO(input);

		if (fryingRecepySO != null) {
			return fryingRecepySO.output;
		}
		return null;
	}

	private FryingRecepySO GetFryingRecepySO(KitchenObjectSO input) {
		foreach (FryingRecepySO fryingRecepySO in fryingRecepySOArray) {
			if (input == fryingRecepySO.input)
				return fryingRecepySO;
		}
		return null;
	}

	private BurningRecepySO GetBurningRecepySO(KitchenObjectSO input) {
		foreach (BurningRecepySO burningRecepySO in burningRecepySOArray) {
			if (input == burningRecepySO.input)
				return burningRecepySO;
		}
		return null;
	}

	public bool IsFried() {
		return state == State.Fried;
	}
}