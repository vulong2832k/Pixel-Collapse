using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("XP")]
    public float finishedXP;
    public float upgradeXP;
    public float xpGain;

    public Action<float, float> OnXPChanged;

    private LevelData _levelData;
    private int _currentUpgradeLevel = 0;

    public LevelLoader levelLoader;

    private void Awake()
    {
        Instance = this;

        if (levelLoader != null)
        {
            levelLoader.OnLevelLoaded += Init;
        }
    }

    void Init()
    {
        _levelData = levelLoader.GetLevelData();

        finishedXP = 0;
        upgradeXP = 0;
        _currentUpgradeLevel = 0;

        OnXPChanged?.Invoke(finishedXP, upgradeXP);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddXP(xpGain);
        }
    }

    public float GetFinishXP()
    {
        if (_levelData == null) return 0;
        return _levelData.scoreToFinish;
    }

    public float GetUpgradeXPRequired()
    {
        if (_levelData == null || _levelData.scoreUpgrades == null || _levelData.scoreUpgrades.Count == 0)
            return 999;

        if (_currentUpgradeLevel >= _levelData.scoreUpgrades.Count)
            return _levelData.scoreUpgrades[_levelData.scoreUpgrades.Count - 1];

        return _levelData.scoreUpgrades[_currentUpgradeLevel];
    }

    public int GetUpgradeLevel()
    {
        return _currentUpgradeLevel;
    }

    public void AddXP(float amount)
    {
        this.finishedXP += amount;
        this.upgradeXP += amount;

        CheckUpgrade();
        CheckFinish();

        OnXPChanged?.Invoke(this.finishedXP, upgradeXP);
    }

    void CheckUpgrade()
    {
        float needXP = GetUpgradeXPRequired();

        if (upgradeXP >= needXP)
        {
            Debug.Log("Level Up");

            upgradeXP -= needXP;
            _currentUpgradeLevel++;
        }
    }

    void CheckFinish()
    {
        if (_levelData == null) return;

        if (finishedXP >= _levelData.scoreToFinish)
        {
            Debug.Log("Victory");
        }
    }
}