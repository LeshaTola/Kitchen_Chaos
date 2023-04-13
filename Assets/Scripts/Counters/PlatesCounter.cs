using System;
using UnityEngine;

public class PlatesCounter : BaseCounter {

	public event EventHandler OnPlateSpawned;
	public event EventHandler OnPlateGrabbed;

	[SerializeField] KitchenObjectSO plateKitchenObjectSO;

	private float plateSpawnTimer;
	private float plateSpawnTimerMax = 4f;
	private float plateAmmount;
	private float plateAmmountMax = 4;

	private void Update() {
		if (GameManager.Instance.IsPlayingTime() && plateAmmount < plateAmmountMax) {
			plateSpawnTimer += Time.deltaTime;
			if (plateSpawnTimer >= plateSpawnTimerMax) {
				plateSpawnTimer = 0;
				plateAmmount++;
				OnPlateSpawned?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public override void Interact(Player player) {
		if (!player.HasKitchenObject()) {
			if (plateAmmount > 0) {
				plateAmmount--;
				OnPlateGrabbed?.Invoke(this, EventArgs.Empty);
				KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
			}
		}
	}
}