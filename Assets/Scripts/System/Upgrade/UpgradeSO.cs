using UnityEngine;

public enum UpgradeType
{
    Damage,
    AttackSpeed,
    Range,
    MultiShot
}

[CreateAssetMenu(menuName = "Game/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    public string description;
    public UpgradeType type;
    public float value;
}