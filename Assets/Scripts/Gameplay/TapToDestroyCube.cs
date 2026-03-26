using UnityEngine;

public class TapToSeparate : MonoBehaviour
{
    [Header("Separate Settings")]
    public float maxForce = 10f;
    public float minForce = 2f;
    public float radius = 3f;

    public LayerMask targetLayer;

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        // Mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTap(touch.position);
            }
        }

        // PC
        if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
    }

    private void HandleTap(Vector2 screenPos)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ApplySeparate(hit.point);
        }
    }

    private void ApplySeparate(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, targetLayer);

        foreach (var hit in hits)
        {
            BreakableCube cube = hit.GetComponent<BreakableCube>();
            if (cube != null)
            {
                float dist = Vector3.Distance(center, cube.transform.position);
                float t = Mathf.Clamp01(dist / radius);

                float force = Mathf.Lerp(maxForce, minForce, t);

                cube.TakeDamage(force, null, DamageType.Separate);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}