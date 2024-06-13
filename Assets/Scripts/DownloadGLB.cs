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

    private string url;
    private string fileName;
    private string filePath;

    public IEnumerator Init(string urlText)
    {
        url = urlText;
        fileName = Path.GetFileName(url); // Extract filename from URL
        // filePath = Path.Combine(Application.temporaryCachePath, fileName);

        string basePath = Path.Combine(Application.dataPath, ".");

        // Create path for DownloadedModels folder
        string downloadedModelsPath = Path.Combine(Application.dataPath, "DownloadedModels");

        filePath = Path.Combine(downloadedModelsPath, fileName);

        if (File.Exists(filePath))
        {
            Debug.Log("GLB already downloaded: " + filePath);
            LoadGLB();
        }
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
        }
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
        string relativeFilePath = "DownloadedModels\\" + fileName;
        // var downloadedObject = AssetDatabase.LoadAssetAtPath<GameObject>(relativeFilePath);
        gLTF.GLTFUri = filePath;
        await gLTF.Load();

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
