using Unity.Netcode;
using UnityEngine;

public class NetworkCannonSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject cannonPrefab;
    [SerializeField]
    private GameObject cannonsLantern;
    [SerializeField]
    private int maxCannons = 60;
    [SerializeField]
    private float cannonPositionXMin = 0f;
    [SerializeField]
    private float cannonPositionXMax = 0f;
    [SerializeField]
    private float cannonPositionZMin = 0f;
    [SerializeField]
    private float cannonPositionZMax = 0f;
    [SerializeField]
    private float cannonPositionY = 0f;


    private void Update()
    {
        if (NetworkManager.IsListening)
        {
            if (IsServer && IsHost)
            {
                for (int i = 0; i < maxCannons; i++)
                {
                    MakeCannon();
                }
            }
        }
    }

    private void MakeCannon()
    {
        if (IsServer && IsHost)
        {
            if (GetCannonNum() <= maxCannons)
            {
                GameObject newCannon = Instantiate(cannonPrefab, new Vector3(Random.Range(cannonPositionXMin, cannonPositionXMax), cannonPositionY, Random.Range(cannonPositionZMin, cannonPositionZMax)), Quaternion.identity);
                newCannon.GetComponent<NetworkObject>().Spawn(true);
                newCannon.transform.parent = cannonsLantern.transform;
                newCannon.GetComponent<CannonController>().SetFireInterval(Random.Range(1, 5));
            }
        }
    }

    private int GetCannonNum()
    {
        return GameObject.FindGameObjectsWithTag("Cannon").Length;
    }
}
