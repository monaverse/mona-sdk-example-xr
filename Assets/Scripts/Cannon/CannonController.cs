using Unity.Netcode;
using UnityEngine;

public class CannonController : NetworkBehaviour
{
    [SerializeField] private GameObject cannonballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fireInterval = 1.0f;
    [SerializeField] private float initialSpeed = 10.0f;

    private float timer;

    private void Start()
    {
        timer = fireInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            FireCannon();
            timer = fireInterval;
        }
    }

    private void FireCannon()
    {
        if (cannonballPrefab != null)
        {
            // Get the direction the cannon is facing
            Vector3 fireDirection;

            // Define the ray origin and direction
            Ray fireRay = new Ray(spawnPoint.position, transform.forward);

            // Perform a ray cast to check for any collisions
            RaycastHit hit;
            if (Physics.Raycast(fireRay, out hit))
            {
                // If there's a collision, use the hit point's normal as the firing direction (adjust based on needs)
                fireDirection = hit.normal;
            }
            else
            {
                // If no collision, use the original forward direction
                fireDirection = transform.up;
            }

            // Instantiate and apply force with the calculated firing direction
            GameObject cannonballInstance = Instantiate(cannonballPrefab, spawnPoint.position, spawnPoint.rotation);
            cannonballInstance.GetComponent<NetworkObject>().Spawn(true);
            cannonballInstance.GetComponent<Rigidbody>().AddForce(fireDirection * initialSpeed, ForceMode.VelocityChange);
        }
    }

    public void SetFireInterval(float interval)
    {
        fireInterval = interval;
    }
}
