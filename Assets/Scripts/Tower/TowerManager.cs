using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    [Header("Global Tower Stats")]
    public float damageMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public int multiShot = 1;

    private void Awake()
    {
        Instance = this;
    }
}