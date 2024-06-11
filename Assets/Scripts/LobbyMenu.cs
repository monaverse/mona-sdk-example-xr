using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Level1-3");
    }

    public void JoinLobby()
    {
        LobbyController lobbyController = GameObject.Find("Lobby").GetComponent<LobbyController>();
        lobbyController.JoinLobby();
    }
}
