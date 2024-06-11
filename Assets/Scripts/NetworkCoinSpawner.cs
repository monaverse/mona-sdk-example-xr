using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class NetworkCoinSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private int startCoins = 3;
    [SerializeField]
    private int maxCoins = 20;
    [SerializeField]
    private float coinPositionXMin = 0f;
    [SerializeField]
    private float coinPositionXMax = 0f;
    [SerializeField]
    private float coinPositionZMin = 0f;
    [SerializeField]
    private float coinPositionZMax = 0f;
    [SerializeField]
    private float coinPositionY = 0f;
    [SerializeField]
    private float creationInterval = 4.0f;


    private float timeSinceLastCreation = 0.0f;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        try
        {
            if (IsServer && IsHost)
            {
                if (GetCoinNum() <= maxCoins)
                {
                    // Make a coin every waitTime seconds
                    timeSinceLastCreation += Time.deltaTime;
                    if (timeSinceLastCreation >= creationInterval)
                    {
                        MakeCoin();
                        timeSinceLastCreation = 0.0f;
                    }
                }
            }
        }
        catch (NotListeningException)
        {
            Debug.Log("Server not listening");
        }
    }

    private void Init()
    {
        if (IsServer && IsHost)
        {
            for (int i = 0; i < startCoins; i++)
            {
                MakeCoin();
            }
        }
    }

    private void MakeCoin()
    {
        if (IsServer && IsHost)
        {
            if (GetCoinNum() <= maxCoins)
            {
                GameObject newCoin = Instantiate(coinPrefab, new Vector3(Random.Range(coinPositionXMin, coinPositionXMax), coinPositionY, Random.Range(coinPositionZMin, coinPositionZMax)), Quaternion.identity);
                newCoin.GetComponent<NetworkObject>().Spawn(true);
            }
        }
    }

    private int GetCoinNum()
    {
        return GameObject.FindGameObjectsWithTag("Coin").Length;
    }
}

