using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
	public event EventHandler InteractEvent;
	public event EventHandler InteractAlternativeEvent;
	public event EventHandler Pause;
	public event EventHandler OnBindingRebind;

	private PlayerController playerController;
	private const string PLAYER_PREFS_BINDING = "Binding";
	public static GameInput Instance { get; private set; }

	public enum Binding
	{
		MoveUp,
		MoveDown,
		MoveLeft,
		MoveRight,
		Interact,
		InteractAlt,
		Pause,
	}

	private void Awake()
	{
		Instance = this;
		playerController = new PlayerController();
		if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDING))
		{
			playerController.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDING));
		}
		playerController.Player.Enable();

		playerController.Player.Interact.performed += Interact_performed;
		playerController.Player.InteractAlternative.performed += InteractAlternative_performed;
		playerController.Player.Pause.performed += Pause_performed;
	}

	private void OnDestroy()
	{
		playerController.Player.Interact.performed -= Interact_performed;
		playerController.Player.InteractAlternative.performed -= InteractAlternative_performed;
		playerController.Player.Pause.performed -= Pause_performed;

		playerController.Dispose();
	}

	private void Pause_performed(InputAction.CallbackContext obj)
	{
		Pause?.Invoke(this, EventArgs.Empty);
	}

	public Vector2 GetMovementNormalized()
	{
		Vector2 inputVector = playerController.Player.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;
		return inputVector;
	}

	private void InteractAlternative_performed(InputAction.CallbackContext obj)
	{
		InteractAlternativeEvent?.Invoke(this, EventArgs.Empty);
	}

	private void Interact_performed(InputAction.CallbackContext obj)
	{
		InteractEvent?.Invoke(this, EventArgs.Empty);
	}

	public string GetBindingText(Binding binding)
	{
		switch (binding)
		{
			default:
			case Binding.Interact:
				return playerController.Player.Interact.bindings[0].ToDisplayString();
			case Binding.InteractAlt:
				return playerController.Player.InteractAlternative.bindings[0].ToDisplayString();
			case Binding.Pause:
				return playerController.Player.Pause.bindings[0].ToDisplayString();
			case Binding.MoveUp:
				return playerController.Player.Move.bindings[1].ToDisplayString();
			case Binding.MoveDown:
				return playerController.Player.Move.bindings[2].ToDisplayString();
			case Binding.MoveLeft:
				return playerController.Player.Move.bindings[3].ToDisplayString();
			case Binding.MoveRight:
				return playerController.Player.Move.bindings[4].ToDisplayString();
		}

	}

	public void RebindBinding(Binding binding, Action OnActionRebound)
	{
		playerController.Player.Disable();
		InputAction inputAction;
		int inputBinding;

		switch (binding)
		{
			default:
			case Binding.Interact:
				inputAction = playerController.Player.Interact;
				inputBinding = 0;
				break;
			case Binding.InteractAlt:
				inputAction = playerController.Player.InteractAlternative;
				inputBinding = 0;
				break;
			case Binding.Pause:
				inputAction = playerController.Player.Pause;
				inputBinding = 0;
				break;
			case Binding.MoveUp:
				inputAction = playerController.Player.Move;
				inputBinding = 1;
				break;
			case Binding.MoveDown:
				inputAction = playerController.Player.Move;
				inputBinding = 2;
				break;
			case Binding.MoveLeft:
				inputAction = playerController.Player.Move;
				inputBinding = 3;
				break;
			case Binding.MoveRight:
				inputAction = playerController.Player.Move;
				inputBinding = 4;
				break;
		}

		inputAction.PerformInteractiveRebinding(inputBinding).OnComplete((callback) =>
		{
			callback.Dispose();
			playerController.Player.Enable();
			OnActionRebound();

			PlayerPrefs.SetString(PLAYER_PREFS_BINDING, playerController.SaveBindingOverridesAsJson());
			PlayerPrefs.Save();

			OnBindingRebind?.Invoke(this, EventArgs.Empty);
		}).Start();
	}
}
