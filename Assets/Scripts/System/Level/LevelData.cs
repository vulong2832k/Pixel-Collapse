using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string towerPrefabName;
    //Level màn chơi
    public int levelNumber;

    //Giới hạn vùng spawn vật thể
    public Vector3 spawnStart;
    public Vector3 spawnEnd;

    //Danh sách có thể spawn
    public List<string> prefabNames = new List<string>();

    //Vị trí các tháp phá khối
    public List<Vector3> towerPos = new List<Vector3>();

    //Vị trí tháp phá khối được đặt sẵn
    public TowerAvailableData towerAvailableData;

    //Yêu cầu màn chơi
    public int scoreToFinish;
    public List<int> scoreUpgrades = new List<int>();
}

