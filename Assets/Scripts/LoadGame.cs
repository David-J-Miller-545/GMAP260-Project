using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public void loadGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
