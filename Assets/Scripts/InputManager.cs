using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameInputActions inputActions;

    // Store lambda references
    private System.Action<InputAction.CallbackContext> onMovePerformed;
    private System.Action<InputAction.CallbackContext> onMoveCanceled;
    private System.Action<InputAction.CallbackContext> onCameraMovePerformed;
    private System.Action<InputAction.CallbackContext> onCameraMoveCanceled;
    private System.Action<InputAction.CallbackContext> onArmPerformed;
    private System.Action<InputAction.CallbackContext> onArmCanceled;
    private System.Action<InputAction.CallbackContext> onPausePerformed;
    private System.Action<InputAction.CallbackContext> onReloadPerformed;
    private System.Action<InputAction.CallbackContext> onFirePerformed;

    public float MoveAxis { get; private set; }
    public float CameraMoveAxis { get; private set; }
    public float ArmMoveAxis { get; private set; }
    public bool PausePressed { get; private set; }
    public bool FirePressed { get; private set; }
    public bool ReloadPressed { get; private set; }

    private void lambdaAssigns()
    {
        // Assign lambdas to fields before subscribing
        onMovePerformed = ctx => MoveAxis = ctx.ReadValue<float>();
        onMoveCanceled = _ => MoveAxis = 0f;

        onCameraMovePerformed = ctx => CameraMoveAxis = ctx.ReadValue<float>();
        onCameraMoveCanceled = _ => CameraMoveAxis = 0f;

        onArmPerformed = ctx => ArmMoveAxis = ctx.ReadValue<float>();
        onArmCanceled = _ => ArmMoveAxis = 0f;

        onReloadPerformed = _ => ReloadPressed = true;
        onFirePerformed = _ => FirePressed = true;


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

        inputActions.Gameplay.CameraMove.performed += onCameraMovePerformed;
        inputActions.Gameplay.CameraMove.canceled += onCameraMoveCanceled;

        inputActions.Gameplay.CatapultArm.performed += onArmPerformed;
        inputActions.Gameplay.CatapultArm.canceled += onArmCanceled;

        inputActions.Gameplay.CatapultReload.performed += onReloadPerformed;
        inputActions.Gameplay.CatapultFire.performed += onFirePerformed;

        inputActions.Gameplay.PauseUnpause.performed += onPausePerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe safely using the stored references
        inputActions.Gameplay.Movement.performed -= onMovePerformed;
        inputActions.Gameplay.Movement.canceled -= onMoveCanceled;

        inputActions.Gameplay.CameraMove.performed -= onCameraMovePerformed;
        inputActions.Gameplay.CameraMove.canceled -= onCameraMoveCanceled;

        inputActions.Gameplay.CatapultArm.performed -= onArmPerformed;
        inputActions.Gameplay.CatapultArm.canceled -= onArmCanceled;

        inputActions.Gameplay.CatapultReload.performed -= onReloadPerformed;
        inputActions.Gameplay.CatapultFire.performed -= onFirePerformed;

        inputActions.Gameplay.PauseUnpause.performed -= onPausePerformed;

        inputActions.Disable();
    }

    private void LateUpdate()
    {
        // Reset one-time presses (so you can check once per frame)
        PausePressed = false;
        ReloadPressed = false;
        FirePressed = false;
    }

    // Optional helpers
    public bool IsMoving => MoveAxis != 0f;

    public bool IsCameraMoving => CameraMoveAxis != 0f;

    public bool IsArmMoving => ArmMoveAxis != 0f;
}
