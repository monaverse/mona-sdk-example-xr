using Unity.Netcode;
using UnityEngine;

public class Cannonball : NetworkBehaviour
{
    [SerializeField] private float initialSpeed = 10.0f;
    [SerializeField] private float airResistance = 0.1f;
    [SerializeField] private float destroyTimeInterval = 1.0f;
    [SerializeField, Range(5f, -10f)] private float groundHeight;

    private Rigidbody rb;
    private Vector3 velocity;
    private float timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocity = transform.forward * initialSpeed;
        timer = destroyTimeInterval;
    }

    private void Update()
    {
        if (transform.position.y < groundHeight)
        {
            Destroy(gameObject);
        }

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // MoveCannonball();
        Decelerate();
    }

    private void MoveCannonball()
    {
        rb.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void Decelerate()
    {
        velocity -= velocity.normalized * airResistance;

        // Optional: Clamp minimum velocity to prevent complete stop due to rounding errors
        velocity = Vector3.ClampMagnitude(velocity, 0.1f);
    }
}
