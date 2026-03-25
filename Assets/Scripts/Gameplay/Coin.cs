using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Coin : MonoBehaviour
{
    private MultiPool _pool;
    private string _poolId;

    private Rigidbody _rb;

    public float force = 2f;
    public float gravityX = 6f;

    public void Init(MultiPool pool, string poolId, Transform attacker)
    {
        _pool = pool;
        _poolId = poolId;

        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        _rb.isKinematic = false;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;

        Vector3 spawnPos = new Vector3(6f, 1.5f, Random.Range(8f, 23f));
        transform.position = spawnPos;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Vector3 dir = attacker != null
            ? (transform.position - attacker.position).normalized
            : Random.insideUnitSphere;

        dir.y = 0.5f;

        _rb.AddForce(dir * force, ForceMode.Impulse);

        if (GetComponent<FakeGravityXCoin>() == null)
        {
            var fake = gameObject.AddComponent<FakeGravityXCoin>();
            fake.gravityX = gravityX;
        }

        Invoke(nameof(ReturnPool), 1f);
    }

    private void ReturnPool()
    {
        if (_pool != null)
        {
            _pool.ReturnToPool(_poolId, gameObject);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}