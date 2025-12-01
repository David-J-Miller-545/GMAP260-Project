using UnityEngine;
using System;

public enum GameState
{
    Live,
    Paused,
    End
}

[CreateAssetMenu(menuName = "Game/Game State")]
public class GameStateSO : ScriptableObject
{
    [SerializeField, Tooltip("The current state of the game.")]
    private GameState currentState = GameState.Live;

    public GameState CurrentState => currentState;

    public event Action<GameState> OnStateChanged;

    public void SetState(GameState newState)
    {
        if (newState == currentState) return;
        if (currentState == GameState.End) return;

        currentState = newState;
        OnStateChanged?.Invoke(currentState);
        Debug.Log($"[GameStateSO] State changed to {currentState}");
    }

    public bool Is(GameState state) => currentState == state;

#if UNITY_EDITOR
    // This helps automatically reset the state when exiting Play Mode
    private void OnDisable()
    {
        currentState = GameState.Live;
    }
#endif
}
