using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FakeGravityX : MonoBehaviour
{
    public float gravity = 10f;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void FixedUpdate()
    {
        if (_rb.isKinematic) return;

        Vector3 vel = _rb.velocity;
        vel.x = -gravity;
        _rb.velocity = vel;

        Vector3 pos = _rb.position;
        _rb.position = pos;

        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
    }
}