using UnityEngine;
using UnityGLTF;

public class GraphicObjectLoader : MonoBehaviour
{
    public GLTFComponent gLTF;

    // Start is called before the first frame update
    private async void Start()
    {
        LobbyManager lobbyManager = GameObject.Find("NetworkManager").GetComponent<LobbyManager>();
        string uri = lobbyManager.GetPlayerLobbyUri();
        gLTF.GLTFUri = uri;
        await gLTF.Load();
    }
}
