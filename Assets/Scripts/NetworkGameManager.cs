using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Services.Lobbies.Models;
using System.Linq;
using Unity.VisualScripting;
using Unity.Collections;

public enum GameState
{
    None,
    Ready,
    Started,
    Playing,
    Complete
}

public struct PlayerGameData
{
    public string playerName;
    public GameState playerState;
    public int playerDisplayId;
    public int playerScore;
}

public class NetworkGameManager : NetworkBehaviour
{
    [SerializeField]
    private ClientUIController clientUI;
    public static NetworkGameManager Instance { get; private set; }

    private NetworkVariable<FixedString512Bytes> url = new NetworkVariable<FixedString512Bytes>();

    private const int MAX_PLAYERS = 4;
    private const int MAX_SCORE = 20;
    Dictionary<ulong, PlayerGameData> playerScores;

    private GameState gameState;
    private GameState previousGameState;

    private void Awake()
    {
        // TODO: Look over game patterns https://gamedevbeginner.com/events-and-delegates-in-unity/
        Instance = this;

        playerScores = new Dictionary<ulong, PlayerGameData>();

        gameState = GameState.None;
        previousGameState = gameState;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSpawned || !IsOwner || !IsServer)
        {
            return;
        }

        foreach (NetworkClient playerClientId in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (!this.playerScores.ContainsKey(playerClientId.PlayerObject.NetworkObjectId))
            {
                if (playerClientId.PlayerObject.IsPlayerObject)
                {
                    Debug.Log("Player Object Network ID " + playerClientId.PlayerObject.NetworkObjectId + " found");
                    SetNewPlayer(playerClientId.PlayerObject.NetworkObjectId);
                }
            }
            else
            {
                PlayerGameData currentGameData = this.playerScores[playerClientId.PlayerObject.NetworkObjectId];
                currentGameData.playerState = GameState.Ready;
                DistributePlayerGameData(playerClientId.PlayerObject.NetworkObjectId, currentGameData.playerName, currentGameData.playerState, currentGameData.playerDisplayId, currentGameData.playerScore);
                DistributePlayerGameDataClientRPC(playerClientId.PlayerObject.NetworkObjectId, currentGameData.playerName, currentGameData.playerState, currentGameData.playerDisplayId, currentGameData.playerScore);
            }
        }

        // Debug.Log("Player count: " + this.playerScores.Count);
        string playerStates = "Ready Up:\n";
        foreach (var entry in this.playerScores)
        {
            playerStates += $"ID: {entry.Key.ToString()} - Status: {entry.Value.playerState.ToString()}\n";
        }
        //Debug.Log(playerStates);

        // If the server finds that all players are in a Ready GameState
        // then the server sets its GameState to Playing and sends a message
        // to the clients to do the same and then the game begins
        if (IsServer)
        {
            this.gameState = GameState.Playing;
            this.previousGameState = GameState.Playing;
            // SetNetworkGameManagerToPlayingClientRPC();
        }

        // If the server finds that a player has reached the final score
        // then the server sets its GameState to Complete and sends a message
        // to the clients to do the same and then the game ends
        if (IsServer && PlayerReachedMaxScore())
        {
            this.gameState = GameState.Complete;
            this.previousGameState = GameState.Complete;
            SetNetworkGameManagerToCompleteClientRPC();
        }
    }

    public Dictionary<ulong, PlayerGameData> GetPlayerGameData()
    {
        return this.playerScores;
    }

    public void DestroyCollectible(ulong networkGameObjectId)
    {
        if (IsOwner)
        {
            DestroyCollectibleServerRPC(networkGameObjectId);
        }
    }

    [ServerRpc]
    private void DestroyCollectibleServerRPC(ulong networkGameObjectId)
    {
        if (IsServer)
        {
            NetworkObject networkGameObject = null;
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkGameObjectId, out networkGameObject))
            {
                networkGameObject.Despawn(true);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeUrlServerRpc(FixedString512Bytes newUrl, ulong id)
    {
        Debug.Log("ChangeUrlServerRpc: " + newUrl + " from " + id);
    }

    public void UpdatePlayerScore(ulong playerId)
    {
        if (IsOwner && IsClient)
        {
            IncrementPlayerScoreServerRPC(playerId);
        }
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public ulong GetPlayerWon()
    {
        int maxScore = 0;
        ulong playerIdWithMaxScore = 99;
        foreach (ulong playerId in this.playerScores.Keys)
        {
            if (this.playerScores[playerId].playerScore > maxScore && this.playerScores[playerId].playerScore >= MAX_SCORE)
            {
                maxScore = this.playerScores[playerId].playerScore;
                playerIdWithMaxScore = playerId;
            }
        }
        return playerIdWithMaxScore;
    }

    public void ResetGame()
    {
        try
        {
            if (IsServer)
            {
                int count = 0;
                while (count < playerScores.Count)
                {
                    KeyValuePair<ulong, PlayerGameData> playerData = playerScores.ElementAt(count);
                    ulong playerId = playerData.Key;
                    PlayerGameData gameData = playerData.Value;
                    gameData.playerScore = 0;
                    gameData.playerState = GameState.Playing;
                    playerScores[playerData.Key] = gameData;
                    DistributeNewPlayerScoreClientRPC(playerId, 0);
                    DistributePlayerGameData(playerId, playerScores[playerId].playerName, playerScores[playerId].playerState, playerScores[playerId].playerDisplayId, playerScores[playerId].playerScore);
                    DistributePlayerGameDataClientRPC(playerId, playerScores[playerId].playerName, playerScores[playerId].playerState, playerScores[playerId].playerDisplayId, playerScores[playerId].playerScore);
                    DistributeNewGameStateClientRPC(GameState.Playing);
                    Debug.Log("Player ID " + playerId + " reset.");
                    count++;
                }
                this.gameState = GameState.Playing;
                this.previousGameState = GameState.Playing;
                DistributeNewGameStateClientRPC(GameState.Playing);
            }
            else if (IsClient)
            {
                Debug.Log("Only the server can reset the game.");
                DistributeNewPlayerScoreClientRPC(this.gameObject.GetComponent<NetworkObject>().NetworkObjectId, 0);
                DistributeNewGameStateClientRPC(GameState.Playing);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void SetNewPlayer(ulong playerId)
    {
        //Debug.Log("NetworkGameManager: SetNewPlayer");
        PlayerGameData gameData = new PlayerGameData();
        this.playerScores.Add(playerId, gameData);

        if (this.playerScores.ContainsKey(playerId))
        {
            //Debug.Log(playerScores.Count);
            //Debug.Log("Player ID " + playerId + " found");
            PlayerGameData savedPlayerGameData = this.playerScores[playerId];
            //Debug.Log("Player ID " + playerId + " added with state " + savedPlayerGameData.playerState);
        }
        else
        {
            Debug.Log("Player ID " + playerId + " not found");
        }

        DistributeNewPlayerGameDataClientRPC(playerId);
    }

    private bool PlayerReachedMaxScore()
    {
        foreach (ulong playerId in playerScores.Keys)
        {
            if (playerScores[playerId].playerScore >= MAX_SCORE)
            {
                return true;
            }
        }
        return false;
    }

    private void DistributePlayerGameData(ulong playerId, string playerName, GameState playerState, int playerDisplayId, int playerScore)
    {
        if (!this.playerScores.ContainsKey(playerId))
        {
            PlayerGameData gameData = new PlayerGameData();
            gameData.playerName = playerName;
            gameData.playerState = playerState;
            gameData.playerDisplayId = playerDisplayId;
            gameData.playerScore = playerScore;
            this.playerScores.Add(playerId, gameData);
        }
        else
        {
            PlayerGameData oldGameData = this.playerScores[playerId];
            oldGameData.playerName = playerName;
            oldGameData.playerState = playerState;
            oldGameData.playerDisplayId = playerDisplayId;
            oldGameData.playerScore = playerScore;
            this.playerScores[playerId] = oldGameData;
        }
    }

    [ClientRpc]
    private void DistributePlayerGameDataClientRPC(ulong playerId, string playerName, GameState playerState, int playerDisplayId, int playerScore)
    {
        if (!this.playerScores.ContainsKey(playerId))
        {
            PlayerGameData gameData = new PlayerGameData();
            gameData.playerName = playerName;
            gameData.playerState = playerState;
            gameData.playerDisplayId = playerDisplayId;
            gameData.playerScore = playerScore;
            this.playerScores.Add(playerId, gameData);
        }
        else
        {
            PlayerGameData oldGameData = this.playerScores[playerId];
            oldGameData.playerName = playerName;
            oldGameData.playerState = playerState;
            oldGameData.playerDisplayId = playerDisplayId;
            oldGameData.playerScore = playerScore;
            this.playerScores[playerId] = oldGameData;
        }
    }

    [ClientRpc]
    private void DistributeNewPlayerGameDataClientRPC(ulong playerId)
    {
        //Debug.Log("Player ID " + playerId + " is a new player.");
        PlayerGameData gameData = new PlayerGameData();
        if (!this.playerScores.ContainsKey(playerId))
        {
            this.playerScores.Add(playerId, gameData);
        }
    }

    [ClientRpc]
    private void SetNetworkGameManagerToCompleteClientRPC()
    {
        this.gameState = GameState.Complete;
        this.previousGameState = GameState.Complete;
    }

    [ClientRpc]
    private void DistributeNewPlayerScoreClientRPC(ulong playerId, int newPlayerScore)
    {
        PlayerGameData gameData = playerScores[playerId];
        gameData.playerScore = newPlayerScore;
        playerScores[playerId] = gameData;
    }

    [ClientRpc]
    private void DistributeNewGameStateClientRPC(GameState newGameState)
    {
        gameState = newGameState;
    }

    [ServerRpc]
    private void IncrementPlayerScoreServerRPC(ulong playerId)
    {
        if (playerScores.ContainsKey(playerId))
        {
            PlayerGameData gameData = playerScores[playerId];
            gameData.playerScore += 1;
            playerScores[playerId] = gameData;
            DistributeNewPlayerScoreClientRPC(playerId, playerScores[playerId].playerScore);
        }
    }
}
