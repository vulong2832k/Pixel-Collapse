using UnityEngine;

public interface ITower
{
    void Attack();
}

public abstract class TowerBase : MonoBehaviour, ITower
{
    [Header("Base Stats")]
    public float baseDamage;
    public float baseAttackRate;
    public float baseRange;

    [SerializeField] protected bool useGlobalUpgrade = true;

    protected float _nextAttackTime;

    protected virtual void Update()
    {
        if (TowerManager.Instance == null) return;

        float finalAttackRate = GetAttackRate();

        if (Time.time >= _nextAttackTime)
        {
            Attack();
            _nextAttackTime = Time.time + finalAttackRate;
        }
    }

    public abstract void Attack();

    protected Collider[] GetTargets()
    {
        return Physics.OverlapSphere(transform.position, GetRange());
    }

    protected void DamageCube(BreakableCube cube)
    {
        if (cube != null)
        {
            cube.TakeDamage(GetDamage());
        }
    }

    // ====== FINAL STATS ======

    protected float GetDamage()
    {
        if (!useGlobalUpgrade || TowerManager.Instance == null)
            return baseDamage;

        return baseDamage * TowerManager.Instance.damageMultiplier;
    }

    protected float GetAttackRate()
    {
        if (!useGlobalUpgrade || TowerManager.Instance == null)
            return baseAttackRate;

        return baseAttackRate * TowerManager.Instance.attackSpeedMultiplier;
    }

    protected float GetRange()
    {
        if (!useGlobalUpgrade || TowerManager.Instance == null)
            return baseRange;

        return baseRange * TowerManager.Instance.rangeMultiplier;
    }
}