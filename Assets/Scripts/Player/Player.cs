using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
	[SerializeField] private float moveSpeed;
	//[SerializeField] private GameInput gameInput;
	[SerializeField] private LayerMask counterLayerMask;
	[SerializeField] private Transform spawnPoint;

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	public class OnSelectedCounterChangedEventArgs : EventArgs { public BaseCounter baseCounter; }
	public static event EventHandler OnPickUpSmth;
	//public static Player Instance { get; private set; }

	private bool isWalking;
	private Vector3 lastDir;
	private BaseCounter baseCounter;
	private KitchenObject kitchenObject;

	/*private void Awake()
	{
		Instance = this;
	}*/

	private void Start()
	{
		GameInput.Instance.InteractEvent += GameInput_interactEvent;
		GameInput.Instance.InteractAlternativeEvent += GameInput_interactAlternativeEvent;
	}

	private void GameInput_interactAlternativeEvent(object sender, EventArgs e)
	{
		if (GameManager.Instance.IsPlayingTime())
		{
			baseCounter?.InteractAlternative(this);
		}
	}

	private void GameInput_interactEvent(object sender, System.EventArgs e)
	{
		if (GameManager.Instance.IsPlayingTime())
		{
			baseCounter?.Interact(this);
		}
	}

	void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		HandleMovement();
		HandleInteract();
	}

	public bool IsWalking()
	{
		return isWalking;
	}

	private void HandleMovement()
	{

		Vector2 inputVector = GameInput.Instance.GetMovementNormalized();

		Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

		float moveDistance = moveSpeed * Time.deltaTime;
		float playerRadius = .65f;
		float playerHeight = 2f;
		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

		if (!canMove)
		{
			Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
			canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

			if (canMove)
			{
				moveDir = moveDirX;//move on x axsis
			}
			else
			{
				Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
				canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

				if (canMove)
				{
					//move on Z axsis
					moveDir = moveDirZ;
				}
			}
		}

		if (canMove)
			transform.position += moveDir * moveDistance;

		isWalking = moveDir != Vector3.zero;

		float rotationSpeed = 10f;
		transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
	}

	public void HandleInteract()
	{
		Vector2 inputVector = GameInput.Instance.GetMovementNormalized();
		Vector3 viewDir = new Vector3(inputVector.x, 0, inputVector.y);

		float interactDistance = 2f;
		if (viewDir != Vector3.zero)
			lastDir = viewDir;

		if (Physics.Raycast(transform.position, lastDir, out RaycastHit raycastHit, interactDistance, counterLayerMask))
			if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
				SetSelectedCounter(baseCounter);
			else
				SetSelectedCounter(null);
		else
			SetSelectedCounter(null);

	}

	private void SetSelectedCounter(BaseCounter baseCounter)
	{
		this.baseCounter = baseCounter;
		OnSelectedCounterChangedEventArgs selectedCounterChangedEventArgs = new OnSelectedCounterChangedEventArgs() { baseCounter = baseCounter };
		OnSelectedCounterChanged?.Invoke(this.baseCounter, selectedCounterChangedEventArgs);
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;
		OnPickUpSmth?.Invoke(this, EventArgs.Empty);
	}

	public KitchenObject GetKitchenObject()
	{
		return kitchenObject;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}
	public Transform GetKitchenObjectFollowTransform()
	{
		return spawnPoint;
	}
}
