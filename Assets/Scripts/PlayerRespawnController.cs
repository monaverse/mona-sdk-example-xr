using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    // The position the player will respawn at
    [SerializeField]
    private Vector3 respawnPosition;
    [SerializeField]
    private float respawnHeight;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < respawnHeight)
        {
            Debug.Log("Player fell off the map, respawning...");
            CharacterController controller = gameObject.GetComponentInParent<CharacterController>();
            controller.enabled = false;
            transform.parent.position = respawnPosition;
            controller.enabled = true;
        }
    }
}
