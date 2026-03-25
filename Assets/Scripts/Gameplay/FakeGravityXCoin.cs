using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FakeGravityXCoin : MonoBehaviour
{
    public float gravityX = 10f;

    private Rigidbody _rb;

    void OnEnable()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (_rb.isKinematic) return;

        _rb.AddForce(Vector3.left * gravityX, ForceMode.Acceleration);
    }
}