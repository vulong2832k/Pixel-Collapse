using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int levelNumber;
    public Vector3 spawnStart;
    public Vector3 spawnEnd;
    public List<string> prefabNames = new List<string>();
    public List<FallingObjectData> fallingObjects = new List<FallingObjectData>();
}

