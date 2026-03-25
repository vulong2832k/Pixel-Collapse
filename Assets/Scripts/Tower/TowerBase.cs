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
        if (TowerManager.Instance == null) return baseDamage;
        return baseDamage * TowerManager.Instance.damageMultiplier;
    }

    protected float GetAttackRate()
    {
        return baseAttackRate / TowerManager.Instance.attackSpeedMultiplier;
    }

    protected float GetRange()
    {
        return baseRange * TowerManager.Instance.rangeMultiplier;
    }
}