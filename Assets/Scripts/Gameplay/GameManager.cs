using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("XP")]
    public float finishedXP;
    public float upgradeXP;
    public float xpGain;

    [Header("Coin")]
    private int _smallXpCounter = 0;
    private int _bigXpCounter = 0;

    //Event
    public Action<float, float> OnXPChanged;
    public Action<int> OnUpgradeLevelUp;

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

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddXP(xpGain);
        }
    }
#endif

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
            upgradeXP -= needXP;
            _currentUpgradeLevel++;

            UpgradeUIManager.Instance.Show();
        }

        OnUpgradeLevelUp?.Invoke(_currentUpgradeLevel);
    }

    void CheckFinish()
    {
        if (_levelData == null) return;

        if (finishedXP >= _levelData.scoreToFinish)
        {
            Debug.Log("Victory");
        }
    }
    public void HandleXPAndCoin(float xp, MultiPool pool, Transform attacker)
    {
        AddXP(xp);

        if (xp <= 8f)
        {
            _smallXpCounter++;
            if (_smallXpCounter >= 5)
            {
                SpawnCoin("coin_small", pool, attacker);
                _smallXpCounter = 0;
            }
        }
        else
        {
            _bigXpCounter++;
            if (_bigXpCounter >= 5)
            {
                SpawnCoin("coin_big", pool, attacker);
                _bigXpCounter = 0;
            }
        }
    }

    private void SpawnCoin(string coinId, MultiPool pool, Transform attacker)
    {
        Vector3 spawnPos = new Vector3(6f, 1.8f, UnityEngine.Random.Range(8f, 23f));

        GameObject coin = pool.GetFromPool(
            coinId,
            spawnPos,
            Quaternion.identity
        );

        if (coin == null) return;

        Coin c = coin.GetComponent<Coin>();
        if (c != null)
        {
            c.Init(pool, coinId, attacker);
        }
    }
}