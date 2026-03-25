using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [Header("Data")]
    public TowerDatabase towerDatabase;
    public List<UpgradeSO> allUpgrades;

    private bool hasGivenStartTower = false;
    private int towerPickCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnUpgradeLevelUp += HandleLevelUp;
    }

    void HandleLevelUp(int level)
    {
        Time.timeScale = 0;

        if (!hasGivenStartTower)
        {
            hasGivenStartTower = true;
            ShowTowerReward();
        }
        else
        {
            ShowUpgradeReward();
        }
    }

    void ShowTowerReward()
    {
        towerPickCount = 0;

        var towers = GetRandomTowers(3);

        Debug.Log("=== CHOOSE 2 TOWERS ===");

        foreach (var t in towers)
        {
            Debug.Log("Tower: " + t.name);
        }

    }

    public void SelectTower(TowerEntry tower)
    {
        TowerUnlockManager.Instance.UnlockTower(tower);
        towerPickCount++;

        Debug.Log("Picked Tower: " + tower.name);

        if (towerPickCount >= 2)
        {
            CloseReward();
        }
    }

    List<TowerEntry> GetRandomTowers(int count)
    {
        List<TowerEntry> list = new List<TowerEntry>(towerDatabase.towers);

        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }

        return list.GetRange(0, count);
    }

    void ShowUpgradeReward()
    {
        var ups = GetRandomUpgrades(3);

        Debug.Log("=== CHOOSE UPGRADE ===");

        foreach (var u in ups)
        {
            Debug.Log("Upgrade: " + u.upgradeName);
        }

    }

    public void SelectUpgrade(UpgradeSO up)
    {
        UpgradeApplier.Apply(up);

        Debug.Log("Picked Upgrade: " + up.upgradeName);

        CloseReward();
    }

    List<UpgradeSO> GetRandomUpgrades(int count)
    {
        List<UpgradeSO> list = new List<UpgradeSO>(allUpgrades);

        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }

        return list.GetRange(0, count);
    }

    void CloseReward()
    {
        Time.timeScale = 1;
    }
}