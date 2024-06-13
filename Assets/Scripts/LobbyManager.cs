using UnityEngine;

namespace Musty
{
    public class LobbyManager : MonoBehaviour
    {
        private LobbyData lobbyData;
        private bool avatarSelected;

        private void Start()
        {
            // Create a new instance of the LobbyData class
            lobbyData = ScriptableObject.CreateInstance<LobbyData>();

            avatarSelected = false;
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
            avatarSelected = true;
        }

        public string GetPlayerDropLobbyUri()
        {
            return lobbyData.GetLobbyPlayerDropUri();
        }

        public void SetPlayerDropLobbyUri(string lobbyUri)
        {
            lobbyData.SetLobbyPlayerDropUri(lobbyUri);
        }

        public bool GetAvatarPicked()
        {
            return avatarSelected;
        }
    }
}
