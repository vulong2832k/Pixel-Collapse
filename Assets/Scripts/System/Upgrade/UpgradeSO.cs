using UnityEngine;

public enum UpgradeType
{
    Damage,
    AttackSpeed,
    Range,
    MultiShot,
    AddTower
}

[CreateAssetMenu(menuName = "Game/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    public string description;
    public UpgradeType type;
    public float value;
    public Sprite icon;
    public GameObject towerPrefab;
}