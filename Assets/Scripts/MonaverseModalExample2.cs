using System.Collections.Generic;
using Monaverse.Api.Modules.Collectibles.Dtos;
using Monaverse.Core.Utils;
using Monaverse.Modal;
using UnityEngine;
using UnityGLTF;
using TMPro;
using Musty;
using Monaverse.Examples;


public class MonaverseModalExample : MonoBehaviour
{
#pragma warning disable CS0436 // Type conflicts with imported type
    [SerializeField] private MonaCollectibleList _compatibleItems;
#pragma warning restore CS0436 // Type conflicts with imported type
    [SerializeField] private MonaCollectibleItemExample _importedItem;
    [SerializeField] private MonaCollectibleItemExample _importedDrop;

    public GLTFComponent gLTF;
    public TMP_Text avatarPickedTextMeshPro;

    private LobbyManager lobbyManager;

    private void Start()
    {
        avatarPickedTextMeshPro.gameObject.SetActive(false);
        MonaverseModal.ImportCollectibleClicked += OnImportCollectibleClicked;
        MonaverseModal.CollectiblesLoaded += OnCollectiblesLoaded;

        lobbyManager = GameObject.Find("NetworkManager").GetComponent<LobbyManager>();
    }

    /// <summary>
    /// Called when a collectibles are loaded in the Monaverse Modal
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="loadedCollectibles">A list of loaded collectibles</param>
    private async void OnCollectiblesLoaded(object sender, List<CollectibleDto> loadedCollectibles)
    {
        Debug.Log("[MonaverseModalExample.OnCollectiblesLoaded] loaded " + loadedCollectibles.Count + " collectibles");
        await _compatibleItems.SetCollectibles(loadedCollectibles);
    }

    /// <summary>
    /// Called when the import button is clicked in a collectible details view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="collectible">The collectible selected for import</param>
    private void OnImportCollectibleClicked(object sender, CollectibleDto collectible)
    {
        string uri = collectible.Versions[collectible.ActiveVersion].Asset;
        Debug.Log("[MonaverseModalExample.OnImportCollectibleClicked] " + collectible.Title);

        if (lobbyManager != null && lobbyManager.GetAvatarPicked())
        {
            // Time to get drop item selection
            _importedDrop.SetCollectible(collectible);
            lobbyManager.SetPlayerDropLobbyUri(uri);
        }
        else
        {
            _importedItem.SetCollectible(collectible);
            lobbyManager.SetPlayerLobbyUri(uri);
            avatarPickedTextMeshPro.gameObject.SetActive(true);
        }
    }
        
    /// <summary>
    /// This is the entry point for the Monaverse Modal
    /// Called on button click
    /// </summary>
    public void OpenModal()
    {
        MonaverseModal.Open();
    }
}
