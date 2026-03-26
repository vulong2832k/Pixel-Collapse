using UnityEngine;

public class TowerCircularSaw : TowerBase
{
    [Header("Saw Settings")]
    public float rotationSpeed = 180f;

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
                cube.TakeDamage(GetDamage(), transform, DamageType.Separate);
            }
        }
    }
    private void RotateSaw()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, GetRange());

            Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
            Gizmos.DrawSphere(transform.position, GetRange());
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, baseRange);
        }
    }
}