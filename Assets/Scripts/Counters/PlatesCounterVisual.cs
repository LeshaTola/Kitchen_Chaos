using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{

	[SerializeField] PlatesCounter platesCounter;
	[SerializeField] Transform spawnPoint;
	[SerializeField] Transform platesVisualPrefabs;

	private List<GameObject> platesVisualGameObjectList;

	private float plateOffsetY = .1f;

	private void Start()
	{
		platesCounter.OnPlateGrabbed += OnPlateGrabbed;
		platesCounter.OnPlateSpawned += OnPlateSpawned;
	}

	private void Awake()
	{
		platesVisualGameObjectList = new List<GameObject>();
	}

	private void OnPlateSpawned(object sender, System.EventArgs e)
	{
		Transform plate = Instantiate(platesVisualPrefabs, spawnPoint);

		plate.transform.localPosition = new Vector3(0, plateOffsetY * platesVisualGameObjectList.Count, 0);

		platesVisualGameObjectList.Add(plate.gameObject);
	}

	private void OnPlateGrabbed(object sender, System.EventArgs e)
	{
		GameObject topPlate = platesVisualGameObjectList[platesVisualGameObjectList.Count - 1];
		platesVisualGameObjectList.Remove(topPlate);
		Destroy(topPlate);
	}
}