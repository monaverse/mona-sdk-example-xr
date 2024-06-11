using System.Collections.Generic;
using Monaverse.Api.Modules.Collectibles.Dtos;
using Monaverse.Core.Utils;
using Monaverse.Modal;
using UnityEngine;
using UnityGLTF;

namespace Monaverse.Examples
{
    public class MonaverseModalExample : MonoBehaviour
    {
        [SerializeField] private MonaCollectibleListExample _compatibleItems;
        [SerializeField] private MonaCollectibleItemExample _importedItem;

        public GLTFComponent gLTF;

        private void Start()
        {
            MonaverseModal.ImportCollectibleClicked += OnImportCollectibleClicked;
            MonaverseModal.CollectiblesLoaded += OnCollectiblesLoaded;
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
        private async void OnImportCollectibleClicked(object sender, CollectibleDto collectible)
        {
            string uri = collectible.Versions[collectible.ActiveVersion].Asset;
            Debug.Log("[MonaverseModalExample.OnImportCollectibleClicked] " + collectible.Title);
            _importedItem.SetCollectible(collectible);
            gLTF.GLTFUri = uri;
            Debug.Log("Getting gltf model from url: " + uri);
            await gLTF.Load();
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
}