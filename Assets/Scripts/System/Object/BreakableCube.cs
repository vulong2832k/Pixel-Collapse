using UnityEngine;

public enum DamageType
{
    Separate,
    Destroy
}

[RequireComponent(typeof(Collider))]
public class BreakableCube : MonoBehaviour
{
    [Header("Health")]
    public float maxHP;
    [SerializeField] private float currentHP;

    [Header("Physics")]
    public float force = .2f;
    public float gravityX = 12f;
    private bool _applyGravityX = false;

    [Header("Coin Spawn")]
    public Vector3 spawnAreaSize;

    private bool _isBroken = false;
    private Rigidbody _rb;

    private MultiPool _pool;
    private string _poolId;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void Init(MultiPool pool, string poolId)
    {
        this._pool = pool;
        this._poolId = poolId;
    }

    void FixedUpdate()
    {
        if (_isBroken && _applyGravityX && _rb != null)
        {
            _rb.AddForce(Vector3.left * gravityX, ForceMode.Acceleration);

            _rb.velocity = new Vector3(Mathf.Clamp(_rb.velocity.x, -5f, 0f), 0f, _rb.velocity.z);

            if (ShouldReturn())
            {
                ReturnSelf();
            }
        }
    }
    private bool ShouldReturn()
    {
        if (transform.position.x < 80f)
            return true;

        if (_rb.velocity.magnitude < 0.05f)
            return true;

        return false;
    }

    public void TakeDamage(float dmg, Transform attacker = null, DamageType type = DamageType.Destroy)
    {
        if (_isBroken && type == DamageType.Destroy) return;

        currentHP -= dmg;

        if (type == DamageType.Destroy)
        {
            if (currentHP <= 0f)
                Break(attacker);
        }
        else
        {
            SeparatePiece(attacker);
        }
    }

    void Break(Transform attacker)
    {
        if (_isBroken || !gameObject.activeInHierarchy) return;
        _isBroken = true;

        GiveXP(attacker);

        transform.parent = null;

        Transform oldParent = transform.parent;

        var runtime = oldParent != null
            ? oldParent.GetComponent<PixelObjectRuntime>()
            : null;

        runtime?.OnCubeRemoved();

        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.mass = 10f;
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.constraints = RigidbodyConstraints.FreezePositionY |
                              RigidbodyConstraints.FreezeRotationX |
                              RigidbodyConstraints.FreezeRotationZ;
        }

        Vector3 dir = Vector3.right;

        if (attacker != null)
        {
            dir = (transform.position - attacker.position).normalized;
            dir.y = 0f;
            dir.Normalize();
        }

        _rb.velocity = Vector3.zero;

        Vector3 forceDir = dir + new Vector3(0f, 0f, Random.Range(-0.3f, 0.3f));
        forceDir.Normalize();

        forceDir.x = -Mathf.Abs(forceDir.x);

        float appliedForce = force * 0.6f;

        _rb.AddForce(forceDir * appliedForce, ForceMode.VelocityChange);

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 2f);

        _applyGravityX = true;
    }
    private void SeparatePiece(Transform attacker)
    {
        if (_isBroken) return;

        Transform oldParent = transform.parent;
        var runtime = oldParent != null ? oldParent.GetComponent<PixelObjectRuntime>() : null;

        transform.parent = null;

        runtime?.OnCubeRemoved();

        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.mass = 5f;
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.constraints = RigidbodyConstraints.FreezePositionY |
                              RigidbodyConstraints.FreezeRotationX |
                              RigidbodyConstraints.FreezeRotationZ;
        }

        Vector3 dir;

        if (attacker != null)
        {
            dir = (transform.position - attacker.position);
        }
        else
        {
            dir = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
        }

        dir.y = 0f;
        dir.Normalize();

        _rb.velocity = Vector3.zero;

        Vector3 forceDir = dir + new Vector3(0f, 0f, Random.Range(-0.3f, 0.3f));
        forceDir.Normalize();

        forceDir.x = -Mathf.Abs(forceDir.x);

        float appliedForce = force * 0.5f;

        _rb.AddForce(forceDir * appliedForce, ForceMode.VelocityChange);

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 2f);

        _applyGravityX = true;
    }

    void ReturnSelf()
    {
        _applyGravityX = false;
        _isBroken = false;

        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        if (_pool != null)
        {
            _pool.ReturnToPool(_poolId, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetCube()
    {
        currentHP = maxHP;
        _isBroken = false;
        _applyGravityX = false;

        if (_rb != null)
        {
            Destroy(_rb);
            _rb = null;
        }
    }

    //XP + COIN LOGIC
    private void GiveXP(Transform attacker)
    {
        if (GameManager.Instance == null) return;

        if (attacker == null || attacker.GetComponent<TowerAvailable>() == null)
            return;

        float xp = Random.Range(5f, 11f);

        GameManager.Instance.HandleXPAndCoin(xp, _pool, attacker);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}