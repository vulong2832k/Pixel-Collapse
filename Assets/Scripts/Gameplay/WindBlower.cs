using UnityEngine;

public class WindBlower : MonoBehaviour
{
    [Header("Wind Settings")]
    public Vector3 windDirection = Vector3.left;
    public float windForce = 5f;
    public bool continuousForce = true;

    [Header("Overlap Box")]
    public Vector3 boxSize = new Vector3(20f, 10f, 20f);

    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, boxSize * 0.5f);
        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Cube"))
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb == null) continue;

                if (continuousForce)
                    rb.AddForce(windDirection.normalized * windForce, ForceMode.Acceleration);
                else
                    rb.velocity = windDirection.normalized * windForce;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}