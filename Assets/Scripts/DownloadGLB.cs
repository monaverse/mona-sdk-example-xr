using Mona.SDK.Brains.Core.Utils;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityGLTF;

public class DownloadGLB : MonoBehaviour
{
    public GLTFComponent gLTF;
    public BrainsGlbLoader _glb;

    private string url;
    private string fileName;
    private string filePath;

    public void Init(string urlText)
    {
        url = urlText;
        fileName = Path.GetFileName(url);
        LoadGLB();
    }

    IEnumerator DownloadFile(string url, string filePath)
    {
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Download error: " + webRequest.error);
                yield break;
            }

            File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
        }
    }

    private async void LoadGLB()
    {
        Debug.Log("load glb");
        gLTF.GLTFUri = url;
        try
        {
            await gLTF.Load();
        }
        catch (Exception ex)
        {
            Debug.LogError($"{ex.Message} {ex.StackTrace}");
            Debug.Log(ex);
            Debug.Log("glb did not load correctly via gltf component");

            try
            {
                _glb.Load(filePath, true, (obj) =>
                {
                    Debug.Log($"Loaded {filePath}");
                    obj.transform.rotation = Quaternion.Euler(0, 180f, 0);
                    obj.transform.parent = this.transform;
                }, 1);
            }
            catch (Exception ex2)
            {
                Debug.LogError($"{ex2.Message}");
                Debug.Log(ex2);
                Debug.Log("glb did not load correctly via glb brains from: " + filePath);

                try
                {
                    _glb.Load(url, true, (obj) =>
                    {
                        Debug.Log($"Loaded {url}");
                        obj.transform.rotation = Quaternion.Euler(0, 180f, 0);
                        obj.transform.parent = this.transform;
                    }, 1);
                }
                catch (Exception ex3)
                {
                    Debug.LogError($"{ex3.Message}");
                    Debug.Log(ex3);
                    Debug.Log("glb did not load correctly via glb brains from: " + url);
                }
            }
        }
    }

    public void LoadDropGLB(string dropItemUri)
    {
        try
        {
            _glb.Load(dropItemUri, true, (obj) =>
            {
                Debug.Log($"Loaded {dropItemUri}");
                obj.transform.rotation = Quaternion.Euler(0, 180f, 0);
                obj.transform.parent = this.transform;
                obj.transform.position = new Vector3 (obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + 5.0f);
            }, 1);
        }
        catch (Exception loadDropGLBException)
        {
            Debug.LogError($"{loadDropGLBException.Message}");
            Debug.Log(loadDropGLBException);
            Debug.Log("glb did not load correctly via glb brains from: " + dropItemUri);
        }
    }
}
