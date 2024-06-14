using Unity.Netcode;
using UnityEngine;

public class NetworkCoinCollector : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            NetworkGameManager.Instance.DestroyCollectible(other.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
            NetworkGameManager.Instance.UpdatePlayerScore(this.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }

        if (other.gameObject.CompareTag("Cannon"))
        {
            Debug.Log("Hit");
        }
    }
}
