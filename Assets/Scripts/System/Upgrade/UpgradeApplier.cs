using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UpgradeApplier
{
    public static void Apply(UpgradeSO up)
    {
        switch (up.type)
        {
            case UpgradeType.Damage:
                TowerManager.Instance.damageMultiplier += up.value;
                break;

            case UpgradeType.AttackSpeed:
                TowerManager.Instance.attackSpeedMultiplier += up.value;
                break;

            case UpgradeType.Range:
                TowerManager.Instance.rangeMultiplier += up.value;
                break;

            case UpgradeType.MultiShot:
                TowerManager.Instance.multiShot += (int)up.value;
                break;
        }
    }
}
