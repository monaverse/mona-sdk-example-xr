using UnityEngine;
using UnityGLTF;

public class GraphicObjectLoader : MonoBehaviour
{
    public GLTFComponent gLTF;

    // Start is called before the first frame update
    private async void Start()
    {
        gLTF.GLTFUri = "https://content.mona.gallery/vppjizac-qheh-loca-grqs-cfbfllx0.vrm";
        await gLTF.Load();
    }
}
