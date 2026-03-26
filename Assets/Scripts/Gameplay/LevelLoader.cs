using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public int totalLevels = 5;
    [SerializeField] private int _currentLevel = 1;   // level hiện tại
    public int CurrentLevel => _currentLevel;

    private LevelData _levelData;

    public TowerDatabase towerDatabase;
    public MultiPool pool;

    [SerializeField] private GameObject _towerPlatformPrefab;
    private List<TowerPlatform> _platforms = new List<TowerPlatform>();

    // Event
    public System.Action OnLevelLoaded;

    private void Start()
    {
        LoadLevelInternal(_currentLevel);
    }

    public bool HasNextLevel() => _currentLevel < totalLevels;

    public void ReloadCurrentLevel()
    {
        LoadLevelInternal(_currentLevel);
    }

    public void LoadNextLevel()
    {
        if (!HasNextLevel()) return;

        _currentLevel++;
        LoadLevelInternal(_currentLevel);
    }

    private void LoadLevelInternal(int levelNum)
    {
        foreach (var obj in FindObjectsOfType<PixelObjectRuntime>())
            Destroy(obj.gameObject);

        foreach (var platform in _platforms)
            Destroy(platform.gameObject);
        _platforms.Clear();

        _currentLevel = levelNum;
        LoadLevel();
        if (_levelData != null)
        {
            LoadMap(_levelData.levelNumber);
            SpawnTowers();

            StopAllCoroutines();
            StartCoroutine(SpawnObjectsRoutine());

            OnLevelLoaded?.Invoke();
        }
        else
        {
            Debug.LogError($"Level {levelNum} không tồn tại!");
        }
    }

    void LoadLevel()
    {
        TextAsset levelJson = Resources.Load<TextAsset>($"Gameplay/Levels/Level_{_currentLevel}");
        if (levelJson == null)
        {
            _levelData = null;
            return;
        }
        _levelData = JsonUtility.FromJson<LevelData>(levelJson.text);
    }

    void LoadMap(int mapNumber)
    {
        TextAsset mapJson = Resources.Load<TextAsset>($"Gameplay/Maps/Map_{mapNumber}");
        if (mapJson == null) return;

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

        foreach (var pos in _levelData.towerPos)
        {
            GameObject obj = Instantiate(_towerPlatformPrefab, pos, Quaternion.identity);
            TowerPlatform platform = obj.GetComponent<TowerPlatform>();
            _platforms.Add(platform);
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
            cube.tag = "Cube";
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

    public LevelData GetLevelData() => _levelData;
}