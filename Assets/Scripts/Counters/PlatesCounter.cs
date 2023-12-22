using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

	public event EventHandler OnPlateSpawned;
	public event EventHandler OnPlateGrabbed;

	[SerializeField] KitchenObjectSO plateKitchenObjectSO;

	private float plateSpawnTimer;
	private float plateSpawnTimerMax = 4f;
	private float plateAmount;
	private float plateAmountMax = 4;

	private void Update()
	{
		if (!IsServer)
		{
			return;
		}

		if (GameManager.Instance.IsPlayingTime() && plateAmount < plateAmountMax)
		{
			plateSpawnTimer += Time.deltaTime;
			if (plateSpawnTimer >= plateSpawnTimerMax)
			{
				plateSpawnTimer = 0;
				SpawnPlateServerRpc();
			}
		}
	}
	[ServerRpc]
	private void SpawnPlateServerRpc()
	{
		SpawnPlateClientRpc();
	}

	[ClientRpc]
	private void SpawnPlateClientRpc()
	{
		plateAmount++;

		OnPlateSpawned?.Invoke(this, EventArgs.Empty);
	}

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject())
		{
			if (plateAmount > 0)
			{
				KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
				InteractServerRpc();
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
		plateAmount--;
		OnPlateGrabbed?.Invoke(this, EventArgs.Empty);
	}
}