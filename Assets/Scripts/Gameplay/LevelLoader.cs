using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public int levelNumber = 1;

    private LevelData _levelData;

    public TowerDatabase towerDatabase;
    public MultiPool pool;

    //Event
    public System.Action OnLevelLoaded;

    void Start()
    {

        LoadLevel();

        if (_levelData == null)
        {
            return;
        }

        LoadMap(_levelData.levelNumber);
        SpawnTowers();

        StartCoroutine(SpawnObjectsRoutine());
        OnLevelLoaded?.Invoke();
    }

    GameObject GetTowerPrefab(string name)
    {
        if (towerDatabase == null) return null;

        foreach (var t in towerDatabase.towers)
        {
            if (t.name == name)
                return t.prefab;
        }

        return null;
    }
    public LevelData GetLevelData()
    {
        return _levelData;
    }
    void LoadLevel()
    {
        TextAsset levelJson = Resources.Load<TextAsset>($"Gameplay/Levels/Level_{levelNumber}");

        if (levelJson == null)
        {
            Debug.LogError("Không tìm thấy Level");
            return;
        }

        _levelData = JsonUtility.FromJson<LevelData>(levelJson.text);
    }

    void LoadMap(int mapNumber)
    {
        TextAsset mapJson = Resources.Load<TextAsset>($"Gameplay/Maps/Map_{mapNumber}");

        if (mapJson == null)
        {
            Debug.LogError("Không tìm thấy Map");
            return;
        }

        PixelMapJSON map = JsonUtility.FromJson<PixelMapJSON>(mapJson.text);

        SpawnMap(map);
    }

    void SpawnMap(PixelMapJSON map)
    {
        foreach (var pixel in map.pixels)
        {
            Vector3 pos = new Vector3(pixel.x + 30, pixel.y, pixel.z);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = pos;

            Renderer r = cube.GetComponent<Renderer>();
            r.material.color = new Color(pixel.r, pixel.g, pixel.b, pixel.a);

            cube.AddComponent<FakeGravityX>();
        }
    }

    void SpawnTowers()
    {
        if (_levelData == null) return;

        GameObject prefab = GetTowerPrefab(_levelData.towerPrefabName);

        if (prefab == null)
            return;

        foreach (var pos in _levelData.towerPos)
        {
            Instantiate(prefab, pos, Quaternion.identity);
        }
    }

    //------------------- SPAWN PIXELOBJECT AUTO -------------------

    private IEnumerator SpawnObjectsRoutine()
    {
        if (_levelData == null || _levelData.prefabNames == null || _levelData.prefabNames.Count == 0)
            yield break;

        while (true)
        {
            string objName = _levelData.prefabNames[Random.Range(0, _levelData.prefabNames.Count)];
            SpawnPixelObject(objName);

            yield return new WaitForSeconds(5f);
        }
    }

    private void SpawnPixelObject(string jsonName)
    {
        TextAsset txt = Resources.Load<TextAsset>($"Prefabs/Objects/{jsonName}");
        if (txt == null) return;

        PixelMapJSON map = JsonUtility.FromJson<PixelMapJSON>(txt.text);

        Vector3 randomPos = new Vector3(
            Random.Range(_levelData.spawnStart.x, _levelData.spawnEnd.x),
            Random.Range(_levelData.spawnStart.y, _levelData.spawnEnd.y),
            Random.Range(_levelData.spawnStart.z, _levelData.spawnEnd.z)
        );

        GameObject parent = new GameObject("PixelObject_" + jsonName);
        parent.transform.position = randomPos;

        Rigidbody rb = parent.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 1000f;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        var runtime = parent.AddComponent<PixelObjectRuntime>();

        Material sharedMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        foreach (var pixel in map.pixels)
        {
            Vector3 localPos = new Vector3(pixel.x, 0, pixel.z);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(parent.transform);
            cube.transform.localPosition = localPos;
            cube.transform.localScale = Vector3.one * 0.95f;
            cube.layer = LayerMask.NameToLayer("Pixel");

            Renderer r = cube.GetComponent<Renderer>();
            r.sharedMaterial = sharedMat;
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_BaseColor", new Color(pixel.r, pixel.g, pixel.b, pixel.a));
            r.SetPropertyBlock(mpb);

            var breakable = cube.AddComponent<BreakableCube>();
            breakable.maxHP = _levelData != null ? _levelData.breakableMaxHP : 10f;
            breakable.Init(pool, "CubePrefab");
            breakable.ResetCube();

            runtime.RegisterCube(new Vector2Int((int)pixel.x, (int)pixel.z), cube);
        }
    }
}