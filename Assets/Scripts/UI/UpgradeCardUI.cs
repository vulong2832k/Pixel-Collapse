using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Button button;
    public Image icon;

    private UpgradeSO _upgrade;
    private TowerEntry _tower;

    public void SetupUpgrade(UpgradeSO up)
    {
        _upgrade = up;
        _tower = null;

        if (up == null)
        {
            Debug.LogError("UPGRADE NULL");
            return;
        }

        title.text = up.upgradeName;
        desc.text = up.description;

        if (icon != null)
            icon.sprite = up.icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void SetupTower(TowerEntry tower)
    {
        _tower = tower;
        _upgrade = null;

        if (tower == null)
        {
            Debug.LogError("TOWER NULL");
            return;
        }

        title.text = tower.name;
        desc.text = "New Tower";

        if (icon != null)
            icon.sprite = tower.icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Debug.Log("CLICK CARD");

        if (_upgrade != null)
        {
            HandleUpgrade();
        }
        else if (_tower != null)
        {
            HandleTower();
        }
        else
        {
            Debug.LogError("NO DATA IN CARD");
        }

        if (UpgradeUIManager.Instance != null)
            UpgradeUIManager.Instance.Hide();
    }

    void HandleUpgrade()
    {
        if (_upgrade.type == UpgradeType.AddTower)
        {
            if (TowerPlacementController.Instance == null)
            {
                Debug.LogError("TowerPlacementController NULL");
                return;
            }

            TowerPlacementController.Instance.StartPlacing(
                _upgrade.towerPrefab,
                (int)_upgrade.value
            );
        }
        else
        {
            UpgradeApplier.Apply(_upgrade);
        }
    }

    void HandleTower()
    {
        if (TowerUnlockManager.Instance == null)
        {
            Debug.LogError("TowerUnlockManager NULL");
            return;
        }

        if (TowerPlacementController.Instance == null)
        {
            Debug.LogError("TowerPlacementController NULL");
            return;
        }

        TowerUnlockManager.Instance.UnlockTower(_tower);

        TowerPlacementController.Instance.StartPlacing(_tower.prefab, 2);
    }
}