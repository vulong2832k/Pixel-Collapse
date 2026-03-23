using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    //Level màn chơi
    public int levelNumber;

    //Giới hạn vùng spawn vật thể
    public Vector3 spawnStart;
    public Vector3 spawnEnd;

    //Danh sách có thể spawn
    public List<string> prefabNames = new List<string>();

    //Vị trí các tháp phá khối
    public List<Vector3> towerPos = new List<Vector3>();

    //Yêu cầu màn chơi
    public int scoreToFinish;
    public List<int> scoreUpgrades = new List<int>();

    //Lượng máu để phá hủy 1 Cube
    public float breakableMaxHP;

    //Tower
    public string towerPrefabName;
    public float towerDamage;
    public float towerAttackRate;
    public float towerRange;

}

