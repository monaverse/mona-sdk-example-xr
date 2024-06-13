using UnityEngine;
// using UnityGLTF;
using Mona.SDK.Brains.Core.Utils;
using Musty;

public class GraphicObjectLoader : MonoBehaviour
{
    // public GLTFComponent gLTF;
   // public BrainsGlbLoader _glb;

    private DownloadGLB _downloadGLB;

    // Start is called before the first frame update
    private void Start()
    {
        _downloadGLB = gameObject.GetComponent<DownloadGLB>();
        LobbyManager lobbyManager = GameObject.Find("NetworkManager").GetComponent<LobbyManager>();
        string uri = lobbyManager.GetPlayerLobbyUri();
        string dropUri = lobbyManager.GetPlayerDropLobbyUri();

        if (uri != null && _downloadGLB != null)
        {
            Debug.Log($"{nameof(GraphicObjectLoader)} {uri}");
            _downloadGLB.Init(uri);
            _downloadGLB.LoadDropGLB(dropUri);
            //StartCoroutine(_downloadGLB.Init(uri));
        }

        // gLTF.GLTFUri = uri;
        // await gLTF.Load();

        /*
        _glb.Load(uri, true, (obj) =>
        {
            Debug.Log($"Loaded {uri}");
            obj.transform.rotation = Quaternion.Euler(0, 180f, 0);
            obj.transform.parent = this.transform;
        }, 1);
        */
    }
}
