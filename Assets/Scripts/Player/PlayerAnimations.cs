using Unity.Netcode;
using UnityEngine;

public class PlayerAnimations : NetworkBehaviour
{
	private Animator animator;
	private const string IS_WALKING = "IsWalking";
	[SerializeField] private Player player;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		animator.SetBool(IS_WALKING, player.IsWalking());
	}
}