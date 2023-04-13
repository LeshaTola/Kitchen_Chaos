using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SounManager : MonoBehaviour {

	[SerializeField] AudioClipsSO audioClipsSO;
	private const string PLAYER_PREFS_SOUND_VOLUME = "SoundEffctVolume";
	public static SounManager Instance{get;private set;}

	private float volume = 1f;

	private void Awake() {
		Instance= this;
		volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME, 1f);
	}

	private void Start() {
		DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
		DeliveryManager.Instance.OnDeliveryFail += DeliveryManager_OnDeliveryFail;
		CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
		Player.OnPickUpSmth += Player_OnPickUpSmth;
		BaseCounter.OnDoropSmth += BaseCounter_OnDoropSmth;
		TrashCounter.OnTrashed += TrashCounter_OnTrashed;
	}

	private void TrashCounter_OnTrashed(object sender, System.EventArgs e) {
		TrashCounter trashCounter = (TrashCounter)sender;
		PlaySound(audioClipsSO.trash, trashCounter.transform.position);
	}

	private void BaseCounter_OnDoropSmth(object sender, System.EventArgs e) {
		BaseCounter baseCounter = (BaseCounter)sender;
		PlaySound(audioClipsSO.drop, baseCounter.transform.position);
	}

	private void Player_OnPickUpSmth(object sender, System.EventArgs e) {
		Player player = (Player)sender;
		PlaySound(audioClipsSO.pickUp, player.transform.position);
	}

	private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
		CuttingCounter cuttingCounter = (CuttingCounter)sender;
		PlaySound(audioClipsSO.chop, cuttingCounter.transform.position);
	}

	private void DeliveryManager_OnDeliveryFail(object sender, System.EventArgs e) {
		DeliveryCounter deliveryCounter =  DeliveryCounter.Instance;
		PlaySound(audioClipsSO.deliveryFail, deliveryCounter.transform.position);
	}

	private void DeliveryManager_OnDeliverySuccess(object sender, System.EventArgs e) {
		DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
		PlaySound(audioClipsSO.deliverySucces, deliveryCounter.transform.position);
	}

	void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
		AudioClip audioClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
		PlaySound(audioClip, position, volume);
	}
	void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplayer = 1f) {
		AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplayer * volume);
	}

	public void PlayFootstepSound(Vector3 position, float volume = 1f) {
		PlaySound(audioClipsSO.footSteps, position, volume);
	}

	public void PlayPopUpSound() {
		PlaySound(audioClipsSO.warning	, Vector3.zero);
	}

	public void PlayWarningSound(Vector3 position) {
		PlaySound(audioClipsSO.warning, position);
	}

	public void ChangeVolume() {
		volume += 0.1f;
		if(volume > 1f) {
			volume = 0f;
		}
		PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_VOLUME, volume);
		PlayerPrefs.Save();
	}

	public float GetVolume() {
		return volume;
	}
}