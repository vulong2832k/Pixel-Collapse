using System.Collections.Generic;
using UnityEngine;

public class TowerUnlockManager : MonoBehaviour
{
    public static TowerUnlockManager Instance;

    public List<TowerEntry> unlockedTowers = new List<TowerEntry>();

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockTower(TowerEntry tower)
    {
        if (!unlockedTowers.Contains(tower))
        {
            unlockedTowers.Add(tower);
        }
    }
}