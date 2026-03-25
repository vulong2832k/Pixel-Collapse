using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Button button;

    private UpgradeSO _upgrade;
    private TowerEntry _tower;

    public void SetupUpgrade(UpgradeSO up)
    {
        _upgrade = up;
        _tower = null;

        title.text = up.upgradeName;
        desc.text = up.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void SetupTower(TowerEntry tower)
    {
        _tower = tower;
        _upgrade = null;

        title.text = tower.name;
        desc.text = "New Tower";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (_upgrade != null)
        {
            UpgradeApplier.Apply(_upgrade);
        }
        else if (_tower != null)
        {
            TowerUnlockManager.Instance.UnlockTower(_tower);
            TowerPlacementController.Instance.StartPlacing(_tower.prefab);
        }

        UpgradeUIManager.Instance.Hide();
    }
}