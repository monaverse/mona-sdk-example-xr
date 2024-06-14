using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class UrlChangedEventArgs
{
    public FixedString512Bytes NewUrl { get; private set; }

    public UrlChangedEventArgs(FixedString512Bytes url)
    {
        NewUrl = url;
    }
}

public delegate void OnUrlChanged(UrlChangedEventArgs e);

public class NetworkedUrls : NetworkBehaviour
{
    public event OnUrlChanged OnUrlUpdated;

    private bool clientCalled = false;
    private FixedString512Bytes clientUrl;

    private void Update()
    {
        if (NetworkGameManager.Instance != null && clientCalled == false)
        {
            NetworkGameManager.Instance.ChangeUrlServerRpc("blank", NetworkObject.NetworkObjectId);
            clientCalled = true;
        }
    }
    // Example of how to change the URL
    public void ChangeUrl(FixedString512Bytes newUrl)
    {
        // url.Value = newUrl;
        // Trigger the custom event
        // OnUrlUpdated?.Invoke(new UrlChangedEventArgs(newUrl));
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn");
        if (IsOwner)
        {
            Debug.Log("OnNetworkSpawn is owner " + NetworkObject.NetworkObjectId.ToString());
        }

        base.OnNetworkSpawn();
    }
}
