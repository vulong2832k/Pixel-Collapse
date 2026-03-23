using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakableCube : MonoBehaviour
{
    [Header("Health")]
    public float maxHP = 10f;
    [SerializeField] private float currentHP;

    [Header("Physics")]
    public float force = 1f;
    public float gravityX = 6f;
    private bool _applyGravityX = false;

    private bool _isBroken = false;
    private Rigidbody _rb;

    void Awake()
    {
        currentHP = maxHP;
    }

    void FixedUpdate()
    {
        if (_isBroken && _applyGravityX && _rb != null)
        {
            Vector3 vel = _rb.velocity;
            if (vel.x > -gravityX)
            {
                vel.x -= gravityX * Time.fixedDeltaTime;
                _rb.velocity = vel;
            }
        }
    }

    public void TakeDamage(float dmg, Transform attacker = null)
    {
        if (_isBroken) return;

        currentHP -= dmg;
        if (currentHP <= 0f)
        {
            Break(attacker);
        }
    }

    void Break(Transform attacker)
    {
        if (_isBroken) return;
        _isBroken = true;

        transform.SetParent(null);

        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 10f;
        _rb.useGravity = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.constraints = RigidbodyConstraints.FreezePositionY |
                          RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationZ;

        Vector3 dir = attacker != null ? (transform.position - attacker.position).normalized : Vector3.up;
        _rb.AddForce(dir * force, ForceMode.Impulse);

        _applyGravityX = true;
    }
}