using System.Collections.Generic;
using UnityEngine;

public class PixelObjectRuntime : MonoBehaviour
{
    public float speed = 4f;

    private Dictionary<Vector2Int, GameObject> _cubes = new();

    public void RegisterCube(Vector2Int pos, GameObject cube)
    {
        _cubes[pos] = cube;
    }

    public GameObject GetCube(Vector2Int pos)
    {
        return _cubes.TryGetValue(pos, out var cube) ? cube : null;
    }
}