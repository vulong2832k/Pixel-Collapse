using System.Collections.Generic;
using UnityEngine;

public static class UpgradeSelector
{
    public static List<UpgradeSO> GetValidUpgrades(List<UpgradeSO> allUpgrades, TowerPlatform[] platforms, int upgradeCount = 3)
    {
        List<UpgradeSO> result = new List<UpgradeSO>();

        int emptyCount = 0;
        foreach (var p in platforms)
            if (!p.IsOccupied) emptyCount++;

        List<UpgradeSO> addTowerUpgrades = allUpgrades.FindAll(u => u.type == UpgradeType.AddTower);
        List<UpgradeSO> otherUpgrades = allUpgrades.FindAll(u => u.type != UpgradeType.AddTower);

        if (emptyCount == platforms.Length)
        {
            Debug.Log($"[UpgradeSelector] Tất cả platform trống ({emptyCount}/{platforms.Length})");
            Debug.Log($"[UpgradeSelector] AddTower count in pool: {addTowerUpgrades.Count}");

            for (int i = 0; i < upgradeCount; i++)
            {
                UpgradeSO addTower = addTowerUpgrades[Random.Range(0, addTowerUpgrades.Count)];
                result.Add(addTower);
                Debug.Log($"[UpgradeSelector] Adding AddTower: {addTower.name}");
            }
        }
        else if (emptyCount > 0)
        {
            // ít nhất 1 platform trống → luôn có 1 AddTower
            if (addTowerUpgrades.Count > 0)
            {
                UpgradeSO randomAddTower = addTowerUpgrades[Random.Range(0, addTowerUpgrades.Count)];
                result.Add(randomAddTower);
            }

            // Thêm các upgrade khác để đủ upgradeCount
            List<UpgradeSO> pool = new List<UpgradeSO>(otherUpgrades.Count + addTowerUpgrades.Count);
            pool.AddRange(otherUpgrades);
            pool.AddRange(addTowerUpgrades); // nếu muốn có thể chọn lại AddTower nhiều lần

            while (result.Count < upgradeCount && pool.Count > 0)
            {
                int idx = Random.Range(0, pool.Count);
                result.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
        }
        else
        {
            //Full platform → chỉ upgrade bình thường
            while (result.Count < upgradeCount && otherUpgrades.Count > 0)
            {
                int idx = Random.Range(0, otherUpgrades.Count);
                result.Add(otherUpgrades[idx]);
                otherUpgrades.RemoveAt(idx);
            }
        }

        return result;
    }
}