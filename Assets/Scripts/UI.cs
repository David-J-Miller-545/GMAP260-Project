using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameStateSO gameState;

    public GameObject pauseScreen;
    public GameObject gameOver;

    void Awake()
    {
        // Optional: Subscribe to state changes
        gameState.OnStateChanged += HandleStateChange;
    }

    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Live:
                pauseScreen.SetActive(false);
                break;
            case GameState.Paused:
                pauseScreen.SetActive(true);
                break;
            case GameState.End:
                gameOver.SetActive(true);
                break;
        }
    }
}
