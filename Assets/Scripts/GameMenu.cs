using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Utilities;

public class GameMenu : MonoBehaviour
{
    public GameObject menuPanel;

    private bool isPaused = false;
    private InputAction escapeAction = null;
    private IDisposable m_EventListener;

    private void Awake()
    {
        escapeAction = new InputAction(binding: "<Keyboard>/escape");
        escapeAction.performed += _ => TogglePause();
        escapeAction.Enable();
    }

    private void Start()
    {
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = false;
    }

    private void OnDisable()
    {
        escapeAction.Disable();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            menuPanel.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
            menuPanel.SetActive(false);
        }
    }

    public void LoadLobby()
    {
        // TODO: Despawn player if not the host
        SceneManager.LoadScene("LobbyScene");
    }

    public void Reset()
    {
        NetworkGameManager.Instance.ResetGame();
    }
}
