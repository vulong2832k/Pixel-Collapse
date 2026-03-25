using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance;

    public CanvasGroup panel;
    public List<UpgradeCardUI> cards;

    public List<UpgradeSO> upgradePool;
    public TowerDatabase towerDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            HideImmediate();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show()
    {
        Time.timeScale = 0f;

        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;

        GenerateCards();
    }

    public void Hide()
    {
        Time.timeScale = 1f;

        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    private void HideImmediate()
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    private void GenerateCards()
    {
        TowerPlatform[] platforms = FindObjectsOfType<TowerPlatform>();
        int upgradeCount = cards.Count;

        List<UpgradeSO> validUpgrades = UpgradeSelector.GetValidUpgrades(upgradePool, platforms, upgradeCount);

        for (int i = 0; i < cards.Count; i++)
        {
            if (i >= validUpgrades.Count)
            {
                cards[i].gameObject.SetActive(false);
                continue;
            }

            UpgradeSO up = validUpgrades[i];

            if (up.type == UpgradeType.AddTower)
            {
                if (towerDatabase.towers.Count > 0)
                {
                    var tower = towerDatabase.towers[Random.Range(0, towerDatabase.towers.Count)];
                    cards[i].SetupTower(tower);
                }
                else
                {
                    cards[i].SetupUpgrade(up);
                }
            }
            else
            {
                cards[i].SetupUpgrade(up);
            }

            cards[i].gameObject.SetActive(true);
        }
    }
}