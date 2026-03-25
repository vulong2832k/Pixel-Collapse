using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakableCube : MonoBehaviour
{
    [Header("Health")]
    public float maxHP;
    [SerializeField] private float currentHP;

    [Header("Physics")]
    public float force = 1f;
    public float gravityX = 6f;
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

        GiveXP(attacker);

        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 10f;
        _rb.useGravity = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.constraints = RigidbodyConstraints.FreezePositionY |
                          RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationZ;

        Vector3 dir = attacker != null
            ? (transform.position - attacker.position).normalized
            : Vector3.up;

        _rb.AddForce(dir * force, ForceMode.Impulse);

        _applyGravityX = true;

        Invoke(nameof(ReturnSelf), 0.15f);
    }

    void ReturnSelf()
    {
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

        float xp = Random.Range(5f, 11f);

        GameManager.Instance.HandleXPAndCoin(xp, _pool, attacker);
    }

    private void SpawnCoin(string coinId, Transform attacker)
    {
        if (_pool == null) return;

        Vector3 spawnPos = GetRandomSpawnPosition();

        GameObject coin = _pool.GetFromPool(
            coinId,
            spawnPos,
            Quaternion.identity
        );

        if (coin == null) return;

        Coin c = coin.GetComponent<Coin>();
        if (c != null)
        {
            c.Init(_pool, coinId, attacker);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = 6f;
        float y = 1.8f;
        float z = Random.Range(8f, 23f);

        return new Vector3(x, y, z);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}