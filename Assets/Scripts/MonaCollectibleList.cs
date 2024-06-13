using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Collectibles.Dtos;
using Monaverse.Redcode.Awaiting;
using UnityEngine;
using Monaverse.Examples;
using Musty;

public class MonaCollectibleList : MonoBehaviour
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private MonaCollectibleItemExample _itemPrefab;
    [SerializeField] private List<MonaCollectibleItemExample> _itemsPool = new();
    [SerializeField] private MonaCollectibleItemExample _selectedItem;
    [SerializeField] private MonaCollectibleItemExample _importedDrop;

    private LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = GameObject.Find("NetworkManager").GetComponent<LobbyManager>();
    }

    public async Task SetCollectibles(List<CollectibleDto> collectibles)
    {
        if (collectibles.Count > _itemsPool.Count)
            await IncreaseCardsPoolSize(collectibles.Count);

        for (var i = 0; i < collectibles.Count; i++)
        {
            var monaCollectibleItem = _itemsPool[i];
            var collectible = collectibles[i];
            monaCollectibleItem.gameObject.SetActive(true);
            monaCollectibleItem.SetCollectible(collectible,
                () =>
                {
                    _importedDrop.SetCollectible(collectible);
                    string uri = collectible.Versions[collectible.ActiveVersion].Asset;
                    lobbyManager.SetPlayerDropLobbyUri(uri);
                });
        }
    }

    private async Task IncreaseCardsPoolSize(int newSize)
    {
        if (newSize <= _itemsPool.Count)
            throw new ArgumentException("New size must be greater than current size");

        var oldSize = _itemsPool.Count;
        _itemsPool.AddRange(new MonaCollectibleItemExample[newSize - oldSize]);

        for (var i = oldSize; i < newSize; i++)
        {
            var card = Instantiate(_itemPrefab, _parent);
            _itemsPool[i] = card;

            // After every 3 new cards, wait for a frame to reduce lag
            if ((i - oldSize + 1) % 3 == 0)
                await new WaitForEndOfFrame();
        }
    }
}