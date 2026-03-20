using UnityEngine;
using System.IO;

public static class MapSerializer
{
    public static void SaveMap(MapData mapData, string path)
    {
        string json = JsonUtility.ToJson(mapData, true);

        string directory = Path.GetDirectoryName(path);
        
        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public static MapData LoadMap(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Map file not found!");
            return null;
        }

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<MapData>(json);
    }
}
