using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTemplate : MonoBehaviour {
	[SerializeField] Image image;

	public void SetIcon(KitchenObjectSO kitchenObjectSO) {
		image.sprite = kitchenObjectSO.sprite;
	}
}
