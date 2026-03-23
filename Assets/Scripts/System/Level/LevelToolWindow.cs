using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SpawnObjectEntry
{
    public enum ObjectType { Prefab, PixelJson }
    public ObjectType type;
    public string path;
    [NonSerialized] public GameObject prefab;
}

public class LevelToolWindow : EditorWindow
{
    private Vector2 _scrollPos;

    [SerializeField] private Material _pixelSharedMaterial;

    // Map
    private string[] _mapFiles;
    private int _selectedMapIndex = 1;
    private string _levelFolder = "Assets/Resources/Gameplay/Levels/";

    // Data
    private LevelData _currentLevelData;

    // Spawn Limit
    private Vector3 _spawnStart;
    private Vector3 _spawnEnd;

    // List Object Spawn
    private List<SpawnObjectEntry> _spawnObjects = new List<SpawnObjectEntry>();
    private string[] _pixelObjectFiles;

    // Tower
    private bool _isPlacingTower = false;

    [MenuItem("Tools/Level Tool")]
    public static void ShowWindow()
    {
        GetWindow<LevelToolWindow>("Level Tool");
    }

    private void OnEnable()
    {
        RefreshMapList();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("=== MAP LIST ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Map List"))
            RefreshMapList();

        if (_mapFiles != null && _mapFiles.Length > 0)
        {
            string[] names = new string[_mapFiles.Length];
            for (int i = 0; i < _mapFiles.Length; i++)
                names[i] = Path.GetFileNameWithoutExtension(_mapFiles[i]);

            _selectedMapIndex = EditorGUILayout.Popup("Select Map", _selectedMapIndex, names);

            if (GUILayout.Button("Create Level From Map") && _selectedMapIndex >= 0)
                LoadMapAndData(_mapFiles[_selectedMapIndex]);
        }

        if (_currentLevelData != null)
        {
            GUILayout.Space(10);
            GUILayout.Label($"=== LEVEL {_currentLevelData.levelNumber} ===", EditorStyles.boldLabel);

            GUILayout.Space(5);
            _currentLevelData.breakableMaxHP = EditorGUILayout.FloatField("Breakable HP", _currentLevelData.breakableMaxHP);

            GUILayout.Space(5);
            DrawSpawn();

            GUILayout.Space(5);
            DrawSpawnObjects();

            GUILayout.Space(5);
            DrawTowerSlots();

            GUILayout.Space(5);
            DrawScore();

            GUILayout.Space(10);
            if (GUILayout.Button("Save Level Data"))
                SaveLevelData();
        }

        EditorGUILayout.EndScrollView();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (_currentLevelData == null) return;

        SceneView.RepaintAll();
        Event e = Event.current;

        // ================= PLACE MODE =================
        if (_isPlacingTower)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            int groundLayer = LayerMask.GetMask("Ground");

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, hit.point, Quaternion.identity, 0.5f, EventType.Repaint);

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Undo.RegisterCompleteObjectUndo(this, "Add Tower Position");
                    _currentLevelData.towerPos.Add(hit.point);
                    Debug.Log("Add Tower Pos: " + hit.point);
                    _isPlacingTower = false;
                    e.Use();
                }
            }

            if (e.type == EventType.MouseDown && e.button == 1)
            {
                _isPlacingTower = false;
                e.Use();
            }
        }

        // ================= DRAW EXISTING =================
        Handles.color = Color.green;
        for (int i = 0; i < _currentLevelData.towerPos.Count; i++)
        {
            Handles.SphereHandleCap(0, _currentLevelData.towerPos[i], Quaternion.identity, 0.4f, EventType.Repaint);
            Handles.Label(_currentLevelData.towerPos[i], $"Tower {i}");
        }
    }

    //-------------------------------------- DRAW UI ------------------------------------------------
    private void DrawSpawn()
    {
        GUILayout.Label("=== SPAWN LIMIT ===", EditorStyles.boldLabel);
        _spawnStart = EditorGUILayout.Vector3Field("Spawn Start", _spawnStart);
        _spawnEnd = EditorGUILayout.Vector3Field("Spawn End", _spawnEnd);
    }

    private void DrawSpawnObjects()
    {
        GUILayout.Label("=== OBJECT SPAWN ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Prefab Slot"))
            _spawnObjects.Add(new SpawnObjectEntry { type = SpawnObjectEntry.ObjectType.Prefab });

        if (GUILayout.Button("Add Pixel JSON Slot"))
            _spawnObjects.Add(new SpawnObjectEntry { type = SpawnObjectEntry.ObjectType.PixelJson });

        RefreshPixelObjectList();

        for (int i = 0; i < _spawnObjects.Count; i++)
        {
            var obj = _spawnObjects[i];
            EditorGUILayout.BeginHorizontal();

            obj.type = (SpawnObjectEntry.ObjectType)EditorGUILayout.EnumPopup(obj.type, GUILayout.Width(100));

            if (obj.type == SpawnObjectEntry.ObjectType.Prefab)
            {
                obj.prefab = (GameObject)EditorGUILayout.ObjectField(obj.prefab, typeof(GameObject), false);
                if (obj.prefab != null) obj.path = obj.prefab.name;
            }
            else if (obj.type == SpawnObjectEntry.ObjectType.PixelJson && _pixelObjectFiles != null && _pixelObjectFiles.Length > 0)
            {
                string[] names = new string[_pixelObjectFiles.Length];
                for (int j = 0; j < _pixelObjectFiles.Length; j++)
                    names[j] = Path.GetFileNameWithoutExtension(_pixelObjectFiles[j]);

                int selectedIndex = Array.IndexOf(names, obj.path);
                if (selectedIndex < 0) selectedIndex = 0;

                selectedIndex = EditorGUILayout.Popup(selectedIndex, names);
                obj.path = names[selectedIndex];

                if (GUILayout.Button("Spawn in Scene"))
                    SpawnPixelObjectInScene(obj.path);
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _spawnObjects.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void RefreshPixelObjectList()
    {
        string folder = Application.dataPath + "/Resources/Prefabs/Objects/";
        if (!Directory.Exists(folder)) return;
        _pixelObjectFiles = Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < _pixelObjectFiles.Length; i++)
            _pixelObjectFiles[i] = _pixelObjectFiles[i].Replace("\\", "/");
    }

    private void DrawTowerSlots()
    {
        GUILayout.Label("=== TOWER POSITIONS ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Slot tower per level"))
            _currentLevelData.towerPos.Add(Vector3.zero);

        if (GUILayout.Button("Place Tower In Scene"))
            _isPlacingTower = true;

        for (int i = 0; i < _currentLevelData.towerPos.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _currentLevelData.towerPos[i] = EditorGUILayout.Vector3Field($"Slot {i}", _currentLevelData.towerPos[i]);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _currentLevelData.towerPos.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(5);
        GUILayout.Label("=== TOWER PREFAB ===", EditorStyles.boldLabel);

        GameObject towerPrefab = null;
        if (!string.IsNullOrEmpty(_currentLevelData.towerPrefabName))
            towerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Resources/Prefabs/Weapon/{_currentLevelData.towerPrefabName}.prefab");

        towerPrefab = (GameObject)EditorGUILayout.ObjectField("Tower Prefab", towerPrefab, typeof(GameObject), false);
        if (towerPrefab != null)
            _currentLevelData.towerPrefabName = towerPrefab.name;
    }

    private void DrawScore()
    {
        if (_currentLevelData.scoreUpgrades == null)
            _currentLevelData.scoreUpgrades = new List<int>();

        while (_currentLevelData.scoreUpgrades.Count < 11)
            _currentLevelData.scoreUpgrades.Add(0);

        GUILayout.Label("=== SCORE ===", EditorStyles.boldLabel);
        _currentLevelData.scoreToFinish = EditorGUILayout.IntField("Score To Finish", _currentLevelData.scoreToFinish);

        GUILayout.Space(5);
        GUILayout.Label("=== SCORE UPGRADE ===", EditorStyles.boldLabel);

        for (int i = 0; i < 11; i++)
        {
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label($"Lv {i + 1}", GUILayout.Width(50));
            _currentLevelData.scoreUpgrades[i] = EditorGUILayout.IntField(_currentLevelData.scoreUpgrades[i]);
            EditorGUILayout.EndHorizontal();
        }
    }
    //-------------------------------------- UTILS ------------------------------------------------
    private void RefreshMapList()
    {
        string folder = Application.dataPath + "/Resources/Gameplay/Maps/";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        _mapFiles = Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories);
        for (int i = 0; i < _mapFiles.Length; i++)
            _mapFiles[i] = _mapFiles[i].Replace("\\", "/");
    }

    private void LoadMapAndData(string mapPath)
    {
        string mapName = Path.GetFileNameWithoutExtension(mapPath);
        int mapNumber = 0;
        string[] parts = mapName.Split('_');
        if (parts.Length > 1) int.TryParse(parts[1], out mapNumber);

        string levelPath = _levelFolder + $"Level_{mapNumber}.json";

        if (File.Exists(levelPath))
        {
            string json = File.ReadAllText(levelPath);
            _currentLevelData = JsonUtility.FromJson<LevelData>(json);
        }
        else
        {
            _currentLevelData = new LevelData { levelNumber = mapNumber };
        }

        if (_currentLevelData.prefabNames == null)
            _currentLevelData.prefabNames = new List<string>();

        if (_currentLevelData.towerPos == null)
            _currentLevelData.towerPos = new List<Vector3>();

        if (_currentLevelData.scoreUpgrades == null)
            _currentLevelData.scoreUpgrades = new List<int>();

        while (_currentLevelData.scoreUpgrades.Count < 11)
            _currentLevelData.scoreUpgrades.Add(0);

        _spawnStart = _currentLevelData.spawnStart;
        _spawnEnd = _currentLevelData.spawnEnd;

        _spawnObjects.Clear();
        foreach (var name in _currentLevelData.prefabNames)
        {
            string jsonPath = $"Assets/Resources/Prefabs/Objects/{name}.json";
            _spawnObjects.Add(new SpawnObjectEntry
            {
                type = File.Exists(jsonPath) ? SpawnObjectEntry.ObjectType.PixelJson : SpawnObjectEntry.ObjectType.Prefab,
                path = name
            });
        }
    }

    private void SaveLevelData()
    {
        if (_currentLevelData == null) return;

        _currentLevelData.spawnStart = _spawnStart;
        _currentLevelData.spawnEnd = _spawnEnd;

        _currentLevelData.prefabNames.Clear();
        foreach (var obj in _spawnObjects)
        {
            if (!string.IsNullOrEmpty(obj.path))
                _currentLevelData.prefabNames.Add(obj.path);
        }

        if (!Directory.Exists(_levelFolder)) Directory.CreateDirectory(_levelFolder);
        string path = _levelFolder + $"Level_{_currentLevelData.levelNumber}.json";
        File.WriteAllText(path, JsonUtility.ToJson(_currentLevelData, true));
        AssetDatabase.Refresh();

        Debug.Log($"Saved Level {_currentLevelData.levelNumber} with {_currentLevelData.prefabNames.Count} objects.");
    }
    private void SpawnPixelObjectInScene(string jsonName)
    {
        TextAsset txt = Resources.Load<TextAsset>($"Prefabs/Objects/{Path.GetFileNameWithoutExtension(jsonName)}");
        if (txt == null)
        {
            Debug.LogError("Không tìm thấy JSON trong Resources: " + jsonName);
            return;
        }

        PixelMapJSON map = JsonUtility.FromJson<PixelMapJSON>(txt.text);

        GameObject parent = new GameObject("PixelObject_" + jsonName);

        if (_pixelSharedMaterial == null)
            _pixelSharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        foreach (var pixel in map.pixels)
        {
            Vector3 pos = new Vector3(pixel.x, 0, pixel.z);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var breakable = cube.AddComponent<BreakableCube>();
            breakable.maxHP = _currentLevelData.breakableMaxHP;

            cube.transform.position = pos;
            cube.transform.parent = parent.transform;

            Renderer r = cube.GetComponent<Renderer>();
            r.sharedMaterial = _pixelSharedMaterial;

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_BaseColor", new Color(pixel.r, pixel.g, pixel.b, pixel.a));
            r.SetPropertyBlock(mpb);
        }

        Undo.RegisterCreatedObjectUndo(parent, "Spawn Pixel Object");
    }
}