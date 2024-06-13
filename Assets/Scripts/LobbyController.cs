using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Authentication;
using TMPro;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Core;
using System;
using Musty;

public class LobbyController : MonoBehaviour
{
    // This script will allow players to interact with the lobby service

    private const int MAX_PLAYERS_IN_LOBBY = 4;

    public GameObject lobbyTextRaw;
    private GameObject lobbyTextPrefab;

    private Lobby joinedLobby;
    private bool isLobbyOwner;
    private string selectedLobbyId;

    // Create a Random object
    private System.Random rand;

    // Create an array of lobby names
    string[] lobbyNames = new string[] {
        "ArenaChampions",
        "GlobalBattlegrounds",
        "MysticQuests",
        "CosmicEncounters",
        "ShadowRealms",
        "EpicExpeditions",
        "FantasyLeagues",
        "VirtualShowdowns",
        "LegendaryTrials",
        "InfiniteAdventures",
        "PixelWarriors",
        "KnightBattles",
        "FutureFrontiers",
        "MysticalJourneys",
        "GalacticArenas",
        "AncientRuins",
        "OceanExplorers",
        "SkyHighAdventures",
        "DesertConquests",
        "FrozenTundras"
    };

    private void Awake()
    {
        rand = new System.Random();

        // Load a ui pabel prefab from the resources folder
        lobbyTextPrefab = Resources.Load<GameObject>("Lobby");
        if (lobbyTextPrefab == null)
        {
            Debug.LogError("Lobby prefab not found in Resources/Prefabs folder");
        }
    }

    private void Start()
    {
        isLobbyOwner = false;

        // A coroutine that will display all the lobbies in the UI every 10 seconds
        StartCoroutine(DisplayAllLobbiesInUICoroutine());
    }

    private IEnumerator DisplayAllLobbiesInUICoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            ClearLobbyGameObjects();
            DisplayAllLobbiesInUI();
        }
    }

    private void ClearLobbyGameObjects()
    {
        GameObject lobbyPanel = GameObject.Find("ScrollPanel");
        foreach (Transform child in lobbyPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private async void DisplayAllLobbiesInUI()
    {
        // Get all the lobbies
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            // Loop through the lobbies and add them to the UI
            foreach (Lobby lobby in lobbies.Results)
            {
                // Check if the lobby is the same as the lobby the player is in
                if (lobby.Id == selectedLobbyId)
                {   
                    bool hasGameStarted = false;
                    try
                    {
                        hasGameStarted = lobby.Data.ContainsKey("GAME_KEY");
                    }
                    catch (NullReferenceException e)
                    {
                        Debug.Log(e);
                    }
                    // Check if the game key is set
                    if (hasGameStarted)
                    {
                        // Check if the player is the owner of the lobby
                        if (isLobbyOwner == false)
                        {
                            // Start the game
                            StartClientWithRelay();
                        }
                    }
                }
                foreach (Player player in lobby.Players)
                {
                    Debug.Log(player.Id);
                }
                AddLobbyToUI(lobby.Name, lobby.Id);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void SetSelectedLobbyId(string lobbyId)
    {
        selectedLobbyId = lobbyId;
    }

    // Create a method that takes a string parameter that will be the lobby name and adds it to a ui panel as a child
    public void AddLobbyToUI(string lobbyName, string lobbyId)
    {
        GameObject lobbyPanel = GameObject.Find("ScrollPanel");
        GameObject lobbyText = Instantiate(lobbyTextRaw, lobbyPanel.transform);
        // Set the text of the Text component to the lobbyName
        lobbyText.GetComponent<TMP_Text>().text = lobbyName;
        lobbyText.transform.localPosition = Vector3.zero;
        GameObject.Find("NetworkManager").GetComponent<LobbyManager>().SetLobbyData(lobbyName, lobbyId);
    }

    public async void JoinLobby()
    {
        if (isLobbyOwner == true)
        {
            Debug.Log("Already own a Lobby");
            return;
        }
        Debug.Log("Joining lobby");
        // Get the selected lobby ID
        string lobbyId = selectedLobbyId;
        // Join the lobby
        try
        {
            await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log($"Joined lobby {lobbyId}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CreateLobby()
    {
        // Get a random integer between 0 and the length of the lobbyNames array
        int index = rand.Next(lobbyNames.Length);
        string lobbyName = lobbyNames[index];
        int maxPlayers = MAX_PLAYERS_IN_LOBBY;
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;

        options.Player = new Player(
            id: AuthenticationService.Instance.PlayerId,
            data: new Dictionary<string, PlayerDataObject>()
            {
                {
                    "ExampleMemberPlayerData",
                    new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: "ExampleMemberPlayerData")
                },
                {
                    "PLAYER_GLB",
                    new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: "ExampleMemberPlayerData")
                }
            }
       );

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        joinedLobby = lobby;
        isLobbyOwner = true;
    }

    public async void StartClientWithRelay()
    {
        // Get lobby with lobby ID and get join code
        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(selectedLobbyId);
        string joinCode = lobby.Data["GAME_KEY"].Value;
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        NetworkManager.Singleton.StartClient();
    }

    public async void StartGame()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3, region: "us-west2");
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "GAME_KEY", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            });

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene("Level1-3", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to start game: {e.Message}");
        }
    }
}

