using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float walkSpeed = 3;
    [SerializeField]
    private float runSpeed = 10;

    [SerializeField]
    private CinemachineFreeLook virtualCamera;

    private Animator animator;

    private PlayerInput controls;
    private InputAction moveAction;
    private InputAction runAction;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controls = GetComponent<PlayerInput>();
        moveAction = controls.actions.FindAction("Move");
        runAction = controls.actions.FindAction("Run");

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

        // Move the player based on input
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Create a Vector3 of the input ignoring the movement in the Y-axis
        Vector3 inputVector = new Vector3(moveInput.x, 0f, moveInput.y);

        float inputMagnitude = Mathf.Clamp01(inputVector.magnitude);
        float currentSpeed = inputMagnitude * speed;

        Vector3 getMoveDir = Quaternion.AngleAxis(virtualCamera.transform.rotation.eulerAngles.y, Vector3.up) * inputVector;
        getMoveDir.Normalize();

        Vector3 velocity = getMoveDir * currentSpeed;

        Vector3 relativePosition = transform.position - virtualCamera.transform.position;
        Quaternion relativePositionNormalized = Quaternion.LookRotation(relativePosition.normalized);
        Quaternion newRotation = new Quaternion(0, relativePositionNormalized.y, 0, relativePositionNormalized.w);

        if (animator != null)
        {
            HandleAnimation(moveInput);
        }

        Move(velocity);
        LookAround(newRotation);
    }

    [ServerRpc]
    private void HandleAnimationServerRPC(Vector2 moveVector)
    {
        HandleAnimation(moveVector);
    }

    private void HandleAnimation(Vector2 moveVector)
    {
        if (moveVector.magnitude > 0)
        {
            animator.SetBool("isWalking", true);
            speed = walkSpeed;
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (moveVector.magnitude >= 1.0f && runAction.ReadValue<float>() > 0)
        {
            animator.SetBool("isRunning", true);
            speed = runSpeed;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void LookAround(Quaternion newRotation)
    {
        transform.rotation = newRotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * 10);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10);
    }

    [ServerRpc]
    private void LookAroundServerRPC(Quaternion newRotation)
    {
        LookAround(newRotation);
    }

    private void Move(Vector3 moveVector)
    {
        controller.Move(moveVector * Time.deltaTime);
    }

    [ServerRpc]
    private void MoveServerRPC(Vector3 moveVector)
    {
        Move(moveVector);
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
