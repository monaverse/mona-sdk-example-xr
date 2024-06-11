using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using System.Linq;

public class ClientUIController : MonoBehaviour
{
    // This script will manage the client's UI and get data from the NetworkGameManager

    public static ClientUIController Instance;

    [SerializeField]
    private TextMeshProUGUI LeaderBoardText;
    [SerializeField]
    private Text wonText;
    [SerializeField]
    private GameObject panel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (NetworkGameManager.Instance != null)
        {
            UpdateScoreBoard(NetworkGameManager.Instance.GetPlayerGameData());
            
            if (NetworkGameManager.Instance.GetGameState() == GameState.Complete)
            {
                LeaderBoardText.text += "Game Over!";
                // wonText.text = "You won!";
                panel.SetActive(true);
                wonText.text = "Player " + NetworkGameManager.Instance.GetPlayerWon() + " Won!";
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }

    public void UpdateScoreBoard(Dictionary<ulong, PlayerGameData> playerScores)
    {
        NetworkGameManager networkGameManager = NetworkGameManager.Instance;
        if (networkGameManager != null)
        {
            var ScoreRanking = "Leaderboard:\n";
            var orderedScores = playerScores.OrderByDescending(x => x.Value.playerScore).ToDictionary(x => x.Key, x => x.Value.playerScore);
            foreach (var entry in orderedScores)
            {
                ScoreRanking += $"ID: {entry.Key.ToString()} Score: {entry.Value.ToString()}\n";
            }
            LeaderBoardText.text = ScoreRanking;
        }
    }
}
