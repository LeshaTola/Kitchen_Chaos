using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgressBar
{

	public event EventHandler<IHasProgressBar.OnProgressChangedEventArgs> OnProgressChanged;
	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

	public class OnStateChangedEventArgs
	{
		public State state;
	}

	public enum State
	{
		Idle,
		Frying,
		Fried,
		Burned
	}

	[SerializeField] private FryingRecepySO[] fryingRecipeSOArray;
	[SerializeField] private BurningRecepySO[] burningRecipeSOArray;

	private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
	private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
	private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);

	private FryingRecepySO fryingRecipeSO;
	private BurningRecepySO burningRecipeSO;

	private void Awake()
	{
		fryingTimer.OnValueChanged += FryingTimerValueChanged;
		burningTimer.OnValueChanged += BurningTimerValueChanged;
		state.OnValueChanged += StateValueChanged;
	}

	private void Update()
	{
		if (!IsServer)
		{
			return;
		}

		if (HasKitchenObject())
		{
			switch (state.Value)
			{
				case State.Idle:
					break;
				case State.Frying:
					fryingTimer.Value += Time.deltaTime;

					if (fryingTimer.Value >= fryingRecipeSO.fryingTimerMax)
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject());

						KitchenObject.SpawnKitchenObject(GetOutputForInput(fryingRecipeSO.input), this);

						fryingTimer.Value = 0;
						state.Value = State.Fried;

						SetBurningRecipeClientRpc(KitchenObjectMultiplayer.Instance.GetIndexOfKitchenObject(GetKitchenObject().GetKitchenObjectSO()));
					}
					break;
				case State.Fried:
					BurningLogicServerRpc();
					break;
				case State.Burned:
					break;
			}
		}
	}

	[ServerRpc]
	private void BurningLogicServerRpc()
	{
		burningTimer.Value += Time.deltaTime;
		BurningLogicClientRpc();
	}

	[ClientRpc]
	private void BurningLogicClientRpc()
	{
		if (burningTimer.Value >= burningRecipeSO.burningTimerMax)
		{
			KitchenObject.DestroyKitchenObject(GetKitchenObject());
			KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

			FinishBurningLogicServerRpc();
		}
	}

	[ServerRpc]
	private void FinishBurningLogicServerRpc()
	{
		burningTimer.Value = 0;
		state.Value = State.Burned;
	}

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			// На стойке Ничего нет
			if (player.HasKitchenObject())
			{
				// У игрока что-то есть
				if (HasOutputWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
				{
					var kitchenObject = player.GetKitchenObject();
					kitchenObject.SetKitchenObjectParent(this);

					InteractPlaceLogicServerRpc(KitchenObjectMultiplayer.Instance.GetIndexOfKitchenObject(kitchenObject.GetKitchenObjectSO()));
				}
			}
		}
		else
		{
			// На стойке Что-то есть
			if (player.HasKitchenObject())
			{
				// У игрока Что-то есть
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
				{
					// И это что-то это тарелка
					if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
						GetKitchenObject().DestroySelf();

					state.Value = State.Idle;
					burningTimer.Value = 0;
					fryingTimer.Value = 0;
				}
			}
			else
			{
				// У игрока ничего нет
				GetKitchenObject().SetKitchenObjectParent(player);

				SetIdleStateServerRpc();
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetIdleStateServerRpc()
	{
		state.Value = State.Idle;
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractPlaceLogicServerRpc(int kitchenObjectSOIndex)
	{
		state.Value = State.Frying;
		burningTimer.Value = 0;
		fryingTimer.Value = 0;

		SetFryingRecipeClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void SetFryingRecipeClientRpc(int kitchenObjectSOIndex)
	{
		var kitchenObjectSO = KitchenObjectMultiplayer.Instance.GetKitchenObject(kitchenObjectSOIndex);

		fryingRecipeSO = GetFryingRecipeSO(kitchenObjectSO);
	}

	[ClientRpc]
	private void SetBurningRecipeClientRpc(int kitchenObjectSOIndex)
	{
		var kitchenObjectSO = KitchenObjectMultiplayer.Instance.GetKitchenObject(kitchenObjectSOIndex);

		burningRecipeSO = GetBurningRecipeSO(kitchenObjectSO);
	}

	private bool HasOutputWithInput(KitchenObjectSO input)
	{
		FryingRecepySO fryingRecipeSO = GetFryingRecipeSO(input);

		if (fryingRecipeSO != null)
		{
			return true;
		}
		return false;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
	{
		FryingRecepySO fryingRecipeSO = GetFryingRecipeSO(input);

		if (fryingRecipeSO != null)
		{
			return fryingRecipeSO.output;
		}
		return null;
	}

	private FryingRecepySO GetFryingRecipeSO(KitchenObjectSO input)
	{
		foreach (FryingRecepySO fryingRecipeSO in fryingRecipeSOArray)
		{
			if (input == fryingRecipeSO.input)
				return fryingRecipeSO;
		}
		return null;
	}

	private BurningRecepySO GetBurningRecipeSO(KitchenObjectSO input)
	{
		foreach (BurningRecepySO burningRecipeSO in burningRecipeSOArray)
		{
			if (input == burningRecipeSO.input)
				return burningRecipeSO;
		}
		return null;
	}

	public bool IsFried()
	{
		return state.Value == State.Fried;
	}

	private void FryingTimerValueChanged(float previousValue, float newValue)
	{
		float fryingTimerMax = fryingRecipeSO == null ? 1f : fryingRecipeSO.fryingTimerMax;

		OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs()
		{
			progressNormalised = newValue / fryingTimerMax
		});
	}

	private void BurningTimerValueChanged(float previousValue, float newValue)
	{
		float burningTimerMax = burningRecipeSO == null ? 1f : burningRecipeSO.burningTimerMax;

		OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs()
		{
			progressNormalised = newValue / burningTimerMax
		});
	}

	private void StateValueChanged(State previousValue, State newValue)
	{
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
		{
			state = newValue
		});

		if (newValue == State.Idle || newValue == State.Burned)
		{
			OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs()
			{
				progressNormalised = 0
			});
		}
	}
}
