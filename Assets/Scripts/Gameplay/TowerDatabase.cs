using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Tower Database")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerEntry> towers;
}

[Serializable]
public class TowerEntry
{
    public string name;
    public GameObject prefab;
    public Sprite icon;
}