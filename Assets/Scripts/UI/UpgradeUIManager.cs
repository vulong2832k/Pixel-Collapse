using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance;

    public GameObject panel;
    public List<UpgradeCardUI> cards;

    public List<UpgradeSO> upgradePool;
    public TowerDatabase towerDatabase;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show()
    {
        Time.timeScale = 0f;
        panel.SetActive(true);

        GenerateCards();
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
    }

    void GenerateCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            bool giveTower = Random.value > 0.5f;

            if (giveTower)
            {
                var tower = towerDatabase.towers[Random.Range(0, towerDatabase.towers.Count)];
                cards[i].SetupTower(tower);
            }
            else
            {
                var up = upgradePool[Random.Range(0, upgradePool.Count)];
                cards[i].SetupUpgrade(up);
            }
        }
    }
}