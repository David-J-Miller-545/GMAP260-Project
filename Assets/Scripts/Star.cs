using UnityEngine;

public class Star : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rock rockScript = other.GetComponent<Rock>();

        if (rockScript != null)
        {
            Debug.Log("Rock touched Star");
            GameStateController.Instance.SetState(GameState.End);
        }
    }
}
