using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class LevelToolWindow : EditorWindow
{
    private Vector2 _scrollPos;
    //Map
    private string[] _mapFiles;
    private int _selectedMapIndex = 1;

    private string _levelFolder = "Assets/Resources/Gameplay/Levels/";

    //Data
    private LevelData _currentLevelData;

    //Spawn Limit
    private Vector3 _spawnStart;
    private Vector3 _spawnEnd;

    //List Prefabs spawn
    private List<GameObject> _objectPrefabs = new List<GameObject>();

    [MenuItem("Tools/Level Tool")]
    public static void ShowWindow()
    {
        GetWindow<LevelToolWindow>("Level Tool");
    }
    private void OnEnable()
    {
        RefreshMapList();
    }
    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUILayout.Label("=== MAP LIST ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Map List"))
            RefreshMapList();

        if (this._mapFiles != null && this._mapFiles.Length > 0)
        {
            string[] names = new string[this._mapFiles.Length];
            for (int i = 0; i < this._mapFiles.Length; i++)
                names[i] = Path.GetFileNameWithoutExtension(this._mapFiles[i]);

            this._selectedMapIndex = EditorGUILayout.Popup("Select Map", this._selectedMapIndex, names);

            if (GUILayout.Button("Create Level From Map"))
            {
                if (this._selectedMapIndex >= 0)
                    LoadMapAndData(_mapFiles[this._selectedMapIndex]);
            }
        }

        if (_currentLevelData != null)
        {
            GUILayout.Space(10);
            GUILayout.Label($"=== LEVEL {_currentLevelData.levelNumber} ===", EditorStyles.boldLabel);

            GUILayout.Space(5);
            DrawSpawn();

            GUILayout.Space(5);
            DrawPrefabs();

            GUILayout.Space(5);
            DrawTowerSlots();

            GUILayout.Space(10);
            DrawAvailableObjects();

            GUILayout.Space(5);
            DrawScore();

            GUILayout.Space(10);
            if (GUILayout.Button("Save Level Data"))
                SaveLevelData();
        }

        EditorGUILayout.EndScrollView();
    }
    //-------------------------------------- DRAW UI ------------------------------------------------
    private void DrawSpawn()
    {
        GUILayout.Label("=== SPAWN LIMIT ===", EditorStyles.boldLabel);
        _spawnStart = EditorGUILayout.Vector3Field("Spawn Start", _spawnStart);
        _spawnEnd = EditorGUILayout.Vector3Field("Spawn End", _spawnEnd);
    }
    private void DrawPrefabs()
    {
        GUILayout.Label("=== OBJECT PREFABS ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Prefab Slot")) 
            this._objectPrefabs.Add(null);

        for (int i = 0; i < _objectPrefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _objectPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(_objectPrefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("X", GUILayout.Width(20))) { _objectPrefabs.RemoveAt(i); i--; }
            EditorGUILayout.EndHorizontal();
        }
    }
    private void DrawTowerSlots()
    {
        GUILayout.Label("=== TOWER POSITIONS ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Slot tower per level"))
        {
            this._currentLevelData.towerPos.Add(Vector3.zero);
        }

        for (int i = 0; i < _currentLevelData.towerPos.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            this._currentLevelData.towerPos[i] = EditorGUILayout.Vector3Field($"Slot {i}", _currentLevelData.towerPos[i]);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                this._currentLevelData.towerPos.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    private void DrawAvailableObjects()
    {
        GUILayout.Label("=== AVAILABLE OBJECT ===", EditorStyles.boldLabel);

        if (_currentLevelData.towerAvailableData == null)
        {
            if (GUILayout.Button("Create Available Object"))
            {
                _currentLevelData.towerAvailableData = new TowerAvailableData();
            }
            return;
        }

        var data = _currentLevelData.towerAvailableData;

        EditorGUILayout.BeginVertical("box");

        GameObject prefab = null;

        if (!string.IsNullOrEmpty(data.prefabName))
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                $"Assets/Prefabs/Objects/{data.prefabName}.prefab");
        }

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (prefab != null)
            data.prefabName = prefab.name;

        data.position = EditorGUILayout.Vector3Field("Position", data.position);
        data.rotationY = EditorGUILayout.FloatField("Rotation Y", data.rotationY);

        if (GUILayout.Button("Remove"))
        {
            _currentLevelData.towerAvailableData = null;
        }

        EditorGUILayout.EndVertical();
    }
    private void DrawScore()
    {
        if (_currentLevelData.scoreUpgrades == null)
            _currentLevelData.scoreUpgrades = new List<int>();

        while (_currentLevelData.scoreUpgrades.Count < 11)
        {
            _currentLevelData.scoreUpgrades.Add(0);
        }

        GUILayout.Label("=== SCORE ===", EditorStyles.boldLabel);

        _currentLevelData.scoreToFinish =
            EditorGUILayout.IntField("Score To Finish", _currentLevelData.scoreToFinish);

        GUILayout.Space(5);
        GUILayout.Label("=== SCORE UPGRADE ===", EditorStyles.boldLabel);

        string[] levelOptions = new string[11];
        for (int i = 0; i < 11; i++)
        {
            EditorGUILayout.BeginHorizontal("box");

            GUILayout.Label($"Lv {i + 1}", GUILayout.Width(50));

            _currentLevelData.scoreUpgrades[i] =
                EditorGUILayout.IntField(_currentLevelData.scoreUpgrades[i]);

            EditorGUILayout.EndHorizontal();
        }
    }
    private void RefreshMapList()
    {
        string folder = Application.dataPath + "/Resources/Gameplay/Maps/";

        if(!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        this._mapFiles = Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories);
        for (int i = 0; i < this._mapFiles.Length; i++)
            this._mapFiles[i] = this._mapFiles[i].Replace("\\", "/");
    }
    private void LoadMapAndData(string mapPath)
    {
        string mapName = Path.GetFileNameWithoutExtension(mapPath);

        int mapNumber = 0;
        string[] parts = mapName.Split('_');

        if (parts.Length > 1)
            int.TryParse(parts[1], out mapNumber);

        string levelPath = _levelFolder + $"Level_{mapNumber}.json";

        // Load hoặc tạo mới
        if (File.Exists(levelPath))
        {
            string json = File.ReadAllText(levelPath);
            _currentLevelData = JsonUtility.FromJson<LevelData>(json);
        }
        else
        {
            _currentLevelData = new LevelData { levelNumber = mapNumber };
        }

        //Null
        if (_currentLevelData.prefabNames == null)
            _currentLevelData.prefabNames = new List<string>();

        if (_currentLevelData.towerPos == null)
            _currentLevelData.towerPos = new List<Vector3>();

        if (_currentLevelData.scoreUpgrades == null)
            _currentLevelData.scoreUpgrades = new List<int>();

        // INIT 11 LEVEL SCORE
        while (_currentLevelData.scoreUpgrades.Count < 11)
        {
            _currentLevelData.scoreUpgrades.Add(0);
        }

        // SYNC GUI
        _spawnStart = _currentLevelData.spawnStart;
        _spawnEnd = _currentLevelData.spawnEnd;

        _objectPrefabs.Clear();

        foreach (var prefabName in _currentLevelData.prefabNames)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(
                $"Assets/Resources/Prefabs/{prefabName}.prefab");

            _objectPrefabs.Add(go);
        }
    }

    private void SaveLevelData()
    {
        if (_currentLevelData == null) return;

        // Đồng bộ dữ liệu
        _currentLevelData.spawnStart = this._spawnStart;
        _currentLevelData.spawnEnd = this._spawnEnd;
        _currentLevelData.prefabNames.Clear();
        foreach (var go in _objectPrefabs)
        {
            if (go != null)
            {
                _currentLevelData.prefabNames.Add(go.name);
            }
        }

        if (!Directory.Exists(_levelFolder))
            Directory.CreateDirectory(_levelFolder);

        string path = _levelFolder + $"Level_{_currentLevelData.levelNumber}.json";
        File.WriteAllText(path, JsonUtility.ToJson(_currentLevelData, true));
        AssetDatabase.Refresh();

        Debug.Log($"Saved Level {_currentLevelData.levelNumber} with {_currentLevelData.prefabNames.Count} prefabs.");
    }
}
