using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameInputActions inputActions;

    // Store lambda references
    private System.Action<InputAction.CallbackContext> onMovePerformed;
    private System.Action<InputAction.CallbackContext> onMoveCanceled;
    private System.Action<InputAction.CallbackContext> onPausePerformed;

    public float MoveAxis { get; private set; }
    public bool PausePressed { get; private set; }

    private void lambdaAssigns()
    {
        // Assign lambdas to fields before subscribing
        onMovePerformed = ctx => MoveAxis = ctx.ReadValue<float>();
        onMoveCanceled = _ => MoveAxis = 0f;
        onPausePerformed = _ => PausePressed = true;
    }

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        lambdaAssigns();

        // Bind input callbacks
        inputActions.Gameplay.Movement.performed += onMovePerformed;
        inputActions.Gameplay.Movement.canceled += onMoveCanceled;
        inputActions.Gameplay.PauseUnpause.performed += onPausePerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe safely using the stored references
        inputActions.Gameplay.Movement.performed -= onMovePerformed;
        inputActions.Gameplay.Movement.canceled -= onMoveCanceled;
        inputActions.Gameplay.PauseUnpause.performed -= onPausePerformed;

        inputActions.Disable();
    }

    private void LateUpdate()
    {
        // Reset one-time presses (so you can check once per frame)
        PausePressed = false;
    }

    // Optional helpers
    public bool IsMoving => MoveAxis != 0f;
}
