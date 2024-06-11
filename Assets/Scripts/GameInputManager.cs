using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }
    public bool menuOpen { get; private set; }
    private PlayerInput playerInput;
    private InputAction menuAndOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            return;
        }

        playerInput = GetComponent<PlayerInput>();
        menuAndOpen = playerInput.actions["OpenAndClose"];
    }

    private void Update()
    {
        menuOpen = menuAndOpen.WasPressedThisFrame();

    }
}
