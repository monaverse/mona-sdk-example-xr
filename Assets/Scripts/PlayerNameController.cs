using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerNameController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            TMP_Text playerText = GameObject.Find("PlayerUI/Background/PlayerName").GetComponent<TMP_Text>();
            if (playerText != null)
            {
                ulong playerId = gameObject.GetComponent<NetworkObject>().NetworkObjectId;
                playerText.text = $"Player {playerId}";
            }
        }
    }
}
