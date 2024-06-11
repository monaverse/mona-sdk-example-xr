using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
