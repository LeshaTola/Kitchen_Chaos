using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "ListOfKitchenObjects")]
public class KitchenObjectListSO : ScriptableObject
{
	[field: SerializeField] public List<KitchenObjectSO> List { get; private set; }
}
