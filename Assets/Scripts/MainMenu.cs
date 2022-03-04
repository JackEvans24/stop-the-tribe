using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene((int)Scenes.Intro);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
