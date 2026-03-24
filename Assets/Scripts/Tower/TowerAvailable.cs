using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TowerAvailable : TowerBase
{
    [Header("Saw Settings")]
    [SerializeField] private float _rotationSpeed = 180f;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    protected override void Update()
    {
        base.Update();
        RotateSaw();
    }

    public override void Attack()
    {
        Collider[] hits = GetTargets();
        foreach (var hit in hits)
        {
            BreakableCube cube = hit.GetComponent<BreakableCube>();
            if (cube != null)
            {
                DamageCube(cube);
            }
        }
    }

    private void RotateSaw()
    {
        transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerStay(Collider other)
    {
        BreakableCube cube = other.GetComponent<BreakableCube>();
        if (cube != null)
        {
            cube.TakeDamage(999f, transform);
        }
    }

    protected new void DamageCube(BreakableCube cube)
    {
        if (cube != null)
        {
            cube.TakeDamage(999f, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, range);
    }
}