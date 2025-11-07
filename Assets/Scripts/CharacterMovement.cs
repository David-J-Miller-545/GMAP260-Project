using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody == null)
        {
            Debug.LogError("rigidbody Rigidbody2D not assigned!");
            enabled = false;
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var input = InputManager.Instance;

        if (input.IsMoving && GameStateController.Instance.Is(GameState.Live))
            Debug.Log($"Moving: {input.MoveAxis}");
            rigidbody.linearVelocityX = input.MoveAxis * 5f;

        if (input.PausePressed)
            if (GameStateController.Instance.Is(GameState.Paused))
            {
                GameStateController.Instance.SetState(GameState.Live);
            }
            else { 
                    GameStateController.Instance.SetState(GameState.Paused); 
                }
            
    }
}
