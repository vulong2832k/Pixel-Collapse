using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WallData
{
    public int x, z;
    public int height;
    public int type;
}
[System.Serializable]
public struct ObjectData
{
    //Position
    public int x;
    public int z;

    //Size
    public int sizeX;
    public int sizeZ;

    //Prefab
    public int prefabIndex;
}
public class MapData
{
    public int width;
    public int height;

    public int[] cellTypes;

    public List<WallData> walls = new List<WallData>();

    public List<ObjectData> objects = new List<ObjectData>();

    public int GetIndex(int x, int z)
    {
        return x + z * width;
    }
}
