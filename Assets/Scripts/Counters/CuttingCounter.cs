using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgressBar
{

	public event EventHandler<IHasProgressBar.OnProgressChangedEventArgs> OnProgressChanged;
	public event EventHandler OnCut;
	public static event EventHandler OnAnyCut;

	new public static void ResetStaticData()
	{
		OnAnyCut = null;
	}

	[SerializeField] private CuttingRecepySO[] cuttingRecepySOArray;

	private int cuttingProgress;
	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			// ���� �� ����� ������ ���
			if (player.HasKitchenObject())
			{
				// � ������ ���-�� ����
				if (HasOutputWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
				{
					// ����������� �������� �� �������
					player.GetKitchenObject().SetKitchenObjectParent(this);

					InteractServerRpc();
				}
			}
		}
		else
		{
			// �� ����� ���-�� ����
			if (player.HasKitchenObject())
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
				{
					// � ��� ���-�� ��� �������
					if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
					}
				}
			}
			else
			{
				GetKitchenObject().SetKitchenObjectParent(player);
				OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs()
				{
					progressNormalised = 0
				});
			}

		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractServerRpc()
	{
		InteractClientRpc();
	}

	[ClientRpc]
	private void InteractClientRpc()
	{
		cuttingProgress = 0;
		OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs()
		{
			progressNormalised = 0f
		});
	}

	public override void InteractAlternative(Player player)
	{
		if (HasKitchenObject() && HasOutputWithInput(GetKitchenObject().GetKitchenObjectSO()))
		{
			InteractAlternativeServerRpc();
			TestFinishingCuttingServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractAlternativeServerRpc()
	{
		InteractAlternativeClientRpc();
	}

	[ClientRpc]
	private void InteractAlternativeClientRpc()
	{
		if (HasKitchenObject() && HasOutputWithInput(GetKitchenObject().GetKitchenObjectSO()))
		{
			cuttingProgress++;

			CuttingRecepySO cuttingRecipeSO = GetCuttingRecepySO(GetKitchenObject().GetKitchenObjectSO());
			OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventArgs()
			{
				progressNormalised = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
			});

			OnCut?.Invoke(this, EventArgs.Empty);
			OnAnyCut?.Invoke(this, EventArgs.Empty);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void TestFinishingCuttingServerRpc()
	{
		if (HasKitchenObject() && HasOutputWithInput(GetKitchenObject().GetKitchenObjectSO()))
		{
			if (cuttingProgress >= GetCuttingRecepySO(GetKitchenObject().GetKitchenObjectSO()).cuttingProgressMax)
			{
				KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
				KitchenObject.DestroyKitchenObject(GetKitchenObject());

				KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
			}
		}
	}

	private bool HasOutputWithInput(KitchenObjectSO input)
	{
		CuttingRecepySO cuttingRecipeSO = GetCuttingRecepySO(input);

		if (cuttingRecipeSO != null)
		{
			return true;
		}
		return false;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
	{
		CuttingRecepySO cuttingRecepySO = GetCuttingRecepySO(input);

		if (cuttingRecepySO != null)
		{
			return cuttingRecepySO.output;
		}
		return null;
	}

	private CuttingRecepySO GetCuttingRecepySO(KitchenObjectSO input)
	{
		foreach (CuttingRecepySO cuttingRecepySO in cuttingRecepySOArray)
		{
			if (input == cuttingRecepySO.input)
				return cuttingRecepySO;
		}
		return null;
	}
}
