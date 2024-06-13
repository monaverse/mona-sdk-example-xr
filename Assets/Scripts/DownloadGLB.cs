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
        fileName = Path.GetFileName(url); // Extract filename from URL
        // filePath = Path.Combine(Application.temporaryCachePath, fileName);

        string basePath = Path.Combine(Application.dataPath, ".");

        // Create path for DownloadedModels folder
       // string downloadedModelsPath = Path.Combine(Application.persistentDataPath, "DownloadedModels");
        // string downloadedModelsPath = Path.Combine(Application.dataPath, "DownloadedModels");
       // Directory.CreateDirectory(downloadedModelsPath);
       // Debug.Log("download path: " + downloadedModelsPath);

        //filePath = Path.Combine(downloadedModelsPath, fileName);

        //if (File.Exists(filePath))
        //{
         //   Debug.Log("GLB already downloaded: " + filePath);
            LoadGLB();
        /*}
        else
        {
            Debug.Log("Downloading GLB: " + url);
            yield return StartCoroutine(DownloadFile(url, filePath));

            if (File.Exists(filePath))
            {
                LoadGLB();
            }
            else
            {
                Debug.LogError("Failed to download GLB file.");
            }
        }*/
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
        Debug.Assert(filePath != null);
        Debug.Log("load glb");
        //string relativeFilePath = "DownloadedModels\\" + fileName;
        // var downloadedObject = AssetDatabase.LoadAssetAtPath<GameObject>(relativeFilePath);
        gLTF.GLTFUri = url;
        try
        {
            //if (File.Exists(relativeFilePath))
            {
                await gLTF.Load();
            }
            else
            {
                Debug.Log("file does not exist");
            }
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

            // var loader = new GLTFSceneImporter(filePath, new ImportOptions());
            // yield return loader.LoadScene(1);

            /*
            if (downloadedObject != null)
            {
                Instantiate(downloadedObject, transform.position, transform.rotation);
            }
            else
            {
                Debug.LogError("Failed to load downloaded GLB file.");
            }
            */
        }
}
