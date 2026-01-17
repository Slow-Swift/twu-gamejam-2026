using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void OnPlayPressed()
    {
        SceneManager.LoadSceneAsync("Main");
    }

    public void OnMainMenuPressed()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
