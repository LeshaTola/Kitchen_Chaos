using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecepySO : ScriptableObject {
	public KitchenObjectSO input;
	public KitchenObjectSO output;
	public int cuttingProgressMax;
}
