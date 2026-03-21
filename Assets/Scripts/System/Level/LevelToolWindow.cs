using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class LevelToolWindow : EditorWindow
{
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
    private int _selectedPrefabIndex = 0;

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

            GUILayout.Label("=== SPAWN LIMIT ===", EditorStyles.boldLabel);
            _spawnStart = EditorGUILayout.Vector3Field("Spawn Start", _spawnStart);
            _spawnEnd = EditorGUILayout.Vector3Field("Spawn End", _spawnEnd);

            GUILayout.Label("=== OBJECT PREFABS ===", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Prefab Slot")) _objectPrefabs.Add(null);

            for (int i = 0; i < _objectPrefabs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _objectPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(_objectPrefabs[i], typeof(GameObject), false);
                if (GUILayout.Button("X", GUILayout.Width(20))) { _objectPrefabs.RemoveAt(i); i--; }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Save Level Data"))
                SaveLevelData(_currentLevelData);
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
        // Lấy số map từ tên file map_{number}.json
        string mapName = Path.GetFileNameWithoutExtension(mapPath);
        int mapNumber = 0;
        string[] parts = mapName.Split('_');
        if (parts.Length > 1)
        {
            int.TryParse(parts[1], out mapNumber);
        }

        //Load Level Data tương ứng với map từ map_{number}
        string levelPath = this._levelFolder + $"level_{mapNumber}.json";

        if (File.Exists(levelPath))
        {
            string json = File.ReadAllText(levelPath);
            this._currentLevelData = JsonUtility.FromJson<LevelData>(json);
        }
        else
        {
            this._currentLevelData = new LevelData { levelNumber = mapNumber };
        }
    }
    private void SaveLevelData(LevelData data)
    {
        // Đồng bộ prefab list
        data.prefabNames.Clear();
        foreach (var go in _objectPrefabs)
            if (go != null) data.prefabNames.Add(go.name);

        // Đồng bộ Spawn Limit
        data.spawnStart = _spawnStart;
        data.spawnEnd = _spawnEnd;

        // Tạo thư mục nếu chưa có
        if (!Directory.Exists(this._levelFolder))
            Directory.CreateDirectory(this._levelFolder);

        // Lưu JSON
        string path = this._levelFolder + $"Level_{data.levelNumber}.json";
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        AssetDatabase.Refresh();

        Debug.Log("Save Data thành công");
    }
}
