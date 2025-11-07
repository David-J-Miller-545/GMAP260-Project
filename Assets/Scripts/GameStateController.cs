using UnityEngine;

public class GameStateController : MonoBehaviour
{
    [Header("Reference to your GameState ScriptableObject")]
    [SerializeField] private GameStateSO gameState;

    public static GameStateController Instance { get; private set; }


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

        if (gameState == null)
        {
            Debug.LogError("GameState ScriptableObject not assigned!");
            enabled = false;
            return;
        }

        // Optional: Subscribe to state changes
        gameState.OnStateChanged += HandleStateChange;
    }

    private void OnDestroy()
    {
        if (gameState != null)
            gameState.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState newState)
    {
        // Example of reacting to state changes
        switch (newState)
        {
            case GameState.Live:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.End:
                // Could trigger UI or scene transition
                break;
        }
    }

    // Public control methods
    public void SetState(GameState newState) => gameState.SetState(newState);
    public bool Is(GameState state) => gameState.Is(state);

    // Example: input-based testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) SetState(GameState.Paused);
        if (Input.GetKeyDown(KeyCode.R)) SetState(GameState.Live);
        if (Input.GetKeyDown(KeyCode.E)) SetState(GameState.End);
    }
}