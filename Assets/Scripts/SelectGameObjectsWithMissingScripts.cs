#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement; //3

public class SelectGameObjectsWithMissingScripts : Editor
{
    [MenuItem("Tools/WPAG Utilities/Select GameObjects With Missing Scripts")]
    static void SelectGameObjects()
    {
        List<GameObject> allObjects = new List<GameObject>();
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>(true))
        {
            allObjects.Add(obj);
        }

        List<Object> objectsWithDeadLinks = new List<Object>();
        foreach (GameObject g in allObjects)
        {
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component currentComponent = components[i];
                if (currentComponent == null)
                {
                    objectsWithDeadLinks.Add(g);
                    Debug.Log(g + " - Find Fucking issue object", g);
                }
            }
        }
        if (objectsWithDeadLinks.Count > 0)
        {
        }
        else
        {
            Debug.Log("Not Found");
        }
    }
}
#endif