using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField]
    private float flySpeed = 10f; // Speed for flying
    [SerializeField]
    private float forwardSpeed = 5f; // Speed for automatic forward movement
    [SerializeField]
    private float initialHeight = 7f; // Initial height for the player
    [SerializeField]
    private CinemachineFreeLook virtualCamera;

    private Animator animator;
    private CharacterController controller;
    private PlayerInput controls;
    private InputAction moveAction;
    private InputAction dropAction;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Playeers forward: " + transform.forward);
        // Set the player's initial height
        transform.position = new Vector3(transform.position.x, initialHeight, transform.position.z);

        // Get the animator component if it exists
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        // Initialize player input
        controls = GetComponent<PlayerInput>();
        moveAction = controls.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        // Read input values for horizontal movement
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Calculate movement direction (left/right) and add automatic forward movement
        Vector3 moveDirection = new Vector3(-forwardSpeed, 0, moveInput.x);
        Vector3 velocity = moveDirection * flySpeed;

        if (animator != null)
        {
            HandleAnimation(moveInput.x != 0 || forwardSpeed != 0);
        }

        Move(velocity);
    }

    private void HandleAnimation(bool isMoving)
    {
        if (isMoving)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        animator.SetBool("isRunning", false); // Ensure running animation is off
    }

    private void Move(Vector3 moveVector)
    {
        // Ensure the player stays at the initial height
        moveVector.y = 0;
        // Move the player
        controller.Move(moveVector * Time.deltaTime);
        // Keep the player at the initial height
        transform.position = new Vector3(transform.position.x, initialHeight, transform.position.z);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            virtualCamera.Priority = 10;
            gameObject.GetComponent<PlayerInput>().enabled = true;
            gameObject.GetComponent<PlayerInput>().ActivateInput();

            DownloadGLB downloadGLB = this.GetComponentInChildren<DownloadGLB>();

            if (downloadGLB != null)
            {
                Debug.Log("downloadGLB available");
                // Do something with the downloadGLB component
                dropAction = new InputAction(binding: "<Gamepad>/buttonSouth");
                dropAction.performed += _ =>
                {
                    Debug.Log("Drop action performed");
                    downloadGLB.DropTheGoods();
                };
                dropAction.Enable();
                Debug.Log("downloadGLB drop action enabled");
            }
            else
            {
                Debug.Log("downloadGLB still null");
            }
        }
        else
        {
            virtualCamera.Priority = 0;
        }
    }
}
