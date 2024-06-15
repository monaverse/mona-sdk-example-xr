using Mona.SDK.Brains.Core.Utils;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityGLTF;
using UnityEngine.InputSystem;

public class DownloadGLB : MonoBehaviour
{
    public GLTFComponent gLTF;
    public BrainsGlbLoader _glb;
    public NetworkedUrls _netUrls;
    public Transform dropLocation;
    public GameObject dropItem;

    private string url;
    private string fileName;
    private string filePath;

    public void DropTheGoods()
    {
        Debug.Log("Drop the goods");
        if (dropItem != null)
        {
            // Make a copy of dropItem, attach a rigidbody, and have the item fall
            // Create a copy of the existing dropItem at the dropPoint's position and rotation
            GameObject droppedItem = Instantiate(dropItem, dropLocation.position, dropLocation.rotation);

            // Ensure the instantiated object has a Rigidbody, add one if it doesn't
            Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = droppedItem.AddComponent<Rigidbody>();
            }

            // Optional: Apply an initial force to the Rigidbody to simulate falling or throwing
            rb.AddForce(Vector3.down * 10f, ForceMode.Impulse); // Adjust the force value as needed

            // Destroy the dropped item after 5 seconds
            Destroy(droppedItem, 5f);
        }
    }

    public void Init(string urlText)
    {
        _netUrls.OnUrlUpdated += OnUrlChange;
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

    private void OnUrlChange(UrlChangedEventArgs e)
    {
        Debug.Log("URL changed to: " + e.NewUrl.ToString());
        
        if (e.NewUrl.ToString() != url)
        {
            Debug.Log("New url, please load the new GLB.");
            LoadExtraGLB(e.NewUrl.ToString());
        }
        else
        {
            Debug.Log("Same url.");
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

        Debug.Log("before change url");
        // _netUrls.GreyUpdate(url);
        Debug.Log("after change url");
    }

    public void LoadDropGLB(string dropItemUri)
    {
        try
        {
            _glb.Load(dropItemUri, true, (obj) =>
            {
                Debug.Log($"Loaded {dropItemUri}");
                obj.transform.rotation = Quaternion.identity;
                obj.transform.parent = dropLocation;
                obj.transform.position = new Vector3(dropLocation.position.x - 2.0f, dropLocation.position.y, dropLocation.position.z);
                dropItem = obj;
            }, 1);
        }
        catch (Exception loadDropGLBException)
        {
            Debug.LogError($"{loadDropGLBException.Message}");
            Debug.Log(loadDropGLBException);
            Debug.Log("glb did not load correctly via glb brains from: " + dropItemUri);
        }
    }

    public void LoadExtraGLB(string extraUri)
    {
        try
        {
            _glb.Load(extraUri, true, (obj) =>
            {
                Debug.Log($"Loaded {extraUri}");
            }, 1);
        }
        catch (Exception loadExtraGLBException)
        {
            Debug.LogError($"{loadExtraGLBException.Message}");
            Debug.Log(loadExtraGLBException);
            Debug.Log("glb did not load extra correctly via glb brains from: " + extraUri);
        }
    }
}
