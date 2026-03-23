using UnityEngine;

public interface ITower
{
    void Attack();
}

public abstract class TowerBase : MonoBehaviour, ITower
{
    [Header("Tower Stats")]
    public float damage = 10f;
    public float attackRate = 0.5f;
    public float range = 5f;

    protected float _nextAttackTime;

    protected virtual void Update()
    {
        if (Time.time >= _nextAttackTime)
        {
            Attack();
            _nextAttackTime = Time.time + attackRate;
        }
    }

    public abstract void Attack();

    protected Collider[] GetTargets()
    {
        return Physics.OverlapSphere(transform.position, range);
    }

    protected void DamageCube(BreakableCube cube)
    {
        if (cube != null)
        {
            cube.TakeDamage(damage, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}