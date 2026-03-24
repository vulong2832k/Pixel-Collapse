using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PixelObjectRuntime : MonoBehaviour
{
    public float speed = 8f;

    private Dictionary<Vector2Int, GameObject> _cubes = new();
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezeRotationX;
    }

    void FixedUpdate()
    {
        _rb.velocity = new Vector3(-speed, 0f, 0f);
    }

    public void RegisterCube(Vector2Int pos, GameObject cube)
    {
        _cubes[pos] = cube;
    }

    public GameObject GetCube(Vector2Int pos)
    {
        return _cubes.TryGetValue(pos, out var cube) ? cube : null;
    }
}