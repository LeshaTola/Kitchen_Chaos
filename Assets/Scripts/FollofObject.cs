using UnityEngine;

public class FollowObject : MonoBehaviour
{
	private Transform followTransform;

	public void SetFollowTransform(Transform transform)
	{
		followTransform = transform;
	}

	private void LateUpdate()
	{
		if (followTransform == null)
		{
			return;
		}

		transform.position = followTransform.position;
		transform.rotation = followTransform.rotation;
	}
}
