using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField]
    private float flySpeed = 10f; // Speed for flying
    [SerializeField]
    private float initialHeight = 7f; // Initial height for the player
    [SerializeField]
    private CinemachineFreeLook virtualCamera;

    private Animator animator;
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        // Set the player's initial height
        transform.position = new Vector3(transform.position.x, initialHeight, transform.position.z);

        // Get the animator component if it exists
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        // Automatically move the player forward in a specified direction (e.g., forward relative to the player)
        Vector3 moveDirection = transform.forward;
        float currentSpeed = flySpeed;

        // Create the velocity vector for flying (including vertical movement)
        Vector3 velocity = moveDirection * currentSpeed;

        if (animator != null)
        {
            HandleAnimation(true); // Pass 'true' to indicate movement
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
        // Move the player upwards slightly to simulate flying
        moveVector.y += flySpeed * Time.deltaTime;
        controller.Move(moveVector * Time.deltaTime);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            virtualCamera.Priority = 10;
            gameObject.GetComponent<PlayerInput>().enabled = true;
            gameObject.GetComponent<PlayerInput>().ActivateInput();
        }
        else
        {
            virtualCamera.Priority = 0;
        }
    }
}
