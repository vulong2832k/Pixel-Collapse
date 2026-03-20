using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ Map Database")]
public class MapDataBase : ScriptableObject
{
    public List<TextAsset> maps = new List<TextAsset>();

    public MapData LoadMap(int index)
    {
        if(index < 0 || index >= maps.Count)
            return null;
        return JsonUtility.FromJson<MapData>(maps[index].text);
    }
}
