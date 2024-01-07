using UnityEngine;

public class PlayerVisual : MonoBehaviour
{

	[SerializeField] private MeshRenderer head;
	[SerializeField] private MeshRenderer body;

	private Material material;

	private void Awake()
	{
		material = new Material(head.material);

		head.material = material;
		body.material = material;
	}

	public void SetPlayerColor(Color color)
	{
		material.color = color;
	}
}
