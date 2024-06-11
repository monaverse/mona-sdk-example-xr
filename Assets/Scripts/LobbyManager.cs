using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private LobbyData lobbyData;

    private void Start()
    {
        // Create a new instance of the LobbyData class
        lobbyData = ScriptableObject.CreateInstance<LobbyData>();
    }

    public void SetLobbyData(string lobbyName, string lobbyId)
    {
        // Set the lobby data
        lobbyData.SetLobbyName(lobbyName);
        lobbyData.SetLobbyId(lobbyId);
    }

    public string GetLobbyDataLobbyId()
    {
        // Return the lobby data
        return lobbyData.GetLobbyId();
    }

    public string GetPlayerLobbyUri()
    {
        return lobbyData.GetLobbyPlayerUri();
    }

    public void SetPlayerLobbyUri(string lobbyUri)
    {
        lobbyData.SetLobbyPlayerUri(lobbyUri);
    }
}
