using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monaverse.Core;
using Monaverse.Modal;
using Monaverse.Api.Modules.Collectibles.Dtos;
using Mona.SDK.Brains.Core.Utils.Structs;
using Mona.SDK.Brains.Core.Utils.Enums;
using System;
using Mona.SDK.Core.Events;
using Unity.VisualScripting;
using Mona.SDK.Core;
using Mona.SDK.Brains.Core;
using Mona.SDK.Brains.Core.Events;
using Mona.SDK.Core.Utils;

public class ConnectExample : MonoBehaviour
{
    private bool _toggled;
    // Start is called before the first frame update

    [SerializeField]
    private GameObject _parent;

    void Start()
    {
        MonaverseModal.ImportCollectibleClicked += HandleSelected;
    }

    public void Open()
    {
        if (!_toggled)
        {
            _parent.gameObject.SetActive(true);
            Monaverse.Modal.MonaverseModal.Open();
        }
        else
        {
            Monaverse.Modal.MonaverseModal.Close();
            _parent.gameObject.SetActive(false);
        }
        _toggled = !_toggled;
    }

    private void HandleSelected(object sender, CollectibleDto item)
    {
        HandleSelect(CopyTo(item, new Token()));
    }

    private Token CopyTo(CollectibleDto nft, Token token)
    {
        token.Id = nft.Id;
        token.Contract = nft.Nft.Contract;
        token.CollectionId = nft.CollectionId;
        token.Description = nft.Description;

        if (!string.IsNullOrEmpty(nft.Image))
        {
            if (nft.Image.Contains("arweave"))
                token.Image = nft.Image;
            else if (!nft.Image.Contains("http"))
                token.Image = nft.Image.CidToIpfsUrl();
        }

        if (token.Image != null && token.Image.Contains("ipfs"))
            token.Image = token.Image.ReplaceIPFS();

        token.Nft.TokenId = nft.Nft.TokenId.ToString();
        //token.Nft.TokenStandard = nft.Nft.;
        token.Nft.TokenUri = nft.Nft.TokenUri;

        token.Nft.Metadata.Id = nft.Id;
        token.Nft.Metadata.Description = nft.Description;
        token.Nft.Metadata.Image = token.Image;
        token.Nft.Metadata.Name = nft.Title;

        token.Traits = new Dictionary<string, object>();

        if (nft.Traits != null)
        {
            foreach (var pair in nft.Traits)
            {
                var traitName = pair.Key.ToLower();
                var traitValue = pair.Value.ToLower();
                if (!token.Traits.ContainsKey(traitName))
                    token.Traits.Add(traitName, traitValue);
            }
        }
        
        var version = nft.Versions[nft.ActiveVersion];

        token.AssetType = (TokenAssetType)Enum.Parse(typeof(TokenAssetType), nft.Type);

        if (token.AssetType == TokenAssetType.Space)
        {
            token.AssetUrl = version.Assets.Space;
        }
        else
        {
            token.AssetUrl = version.Asset;
        }

        token.Artist = nft.Creator;

        Debug.Log($"{nameof(ConnectExample)}.parse token url: {token.AssetUrl} image: {token.Image}");

        return token;
    }

    private Action<MonaChangeAvatarEvent> OnChangeAvatar;
    private Action<MonaChangeSpaceEvent> OnChangeSpace;
    private Action<MonaChangeSpawnEvent> OnChangeSpawn;

    private void HandleSelect(Token token)
    {
        OnChangeAvatar = HandleChangeAvatar;
        MonaEventBus.Register<MonaChangeAvatarEvent>(new EventHook(MonaCoreConstants.ON_CHANGE_AVATAR_EVENT), OnChangeAvatar);

        OnChangeSpace = HandleChangeSpace;
        MonaEventBus.Register<MonaChangeSpaceEvent>(new EventHook(MonaCoreConstants.ON_CHANGE_SPACE_EVENT), OnChangeSpace);

        OnChangeSpawn = HandleChangeSpawn;
        MonaEventBus.Register<MonaChangeSpawnEvent>(new EventHook(MonaCoreConstants.ON_CHANGE_SPAWN_EVENT), OnChangeSpawn);

        Debug.Log($"{nameof(HandleSelect)} {token.Nft.Metadata.Name} {token.AssetUrl}");
        MonaEventBus.Trigger<MonaWalletTokenSelectedEvent>(new EventHook(MonaBrainConstants.WALLET_TOKEN_SELECTED_EVENT), new MonaWalletTokenSelectedEvent(token));
    }

    private void HandleChangeAvatar(MonaChangeAvatarEvent evt)
    {

    }

    private void HandleChangeSpace(MonaChangeSpaceEvent evt)
    {

    }

    private void HandleChangeSpawn(MonaChangeSpawnEvent evt)
    {

    }
}
