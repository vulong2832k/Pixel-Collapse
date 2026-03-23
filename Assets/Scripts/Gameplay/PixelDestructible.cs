using UnityEngine;

public class PixelDestructible : MonoBehaviour
{
    public void BreakCube(GameObject cube)
    {
        if (cube == null) return;

        cube.transform.SetParent(null);

        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var fg = cube.AddComponent<FakeGravityX>();
        fg.gravity = 4f;

        rb.AddForce(
            new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 3f), 0),
            ForceMode.Impulse
        );

        if (transform.childCount <= 1)
        {
            Destroy(gameObject);
        }
    }
}