using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public static class Utility 
{
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static T GetJsonObject<T>(string filename)
    {
        string folder = @"DataFiles/temp";
        string extension = ".txt";

        string relativePath = Path.Combine("Assets", folder, filename + extension);

        if (File.Exists(relativePath))
        {
            string json = File.ReadAllText(relativePath);
            return JsonUtility.FromJson<T>(json);
        }
        else
        {
            Debug.LogWarning($"File with path \"{relativePath}\" not found");
        }
        return default;
    }

}
