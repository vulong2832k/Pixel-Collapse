using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PixelData
{
    public float x, y, z;
    public float r, g, b, a;
}

[Serializable]
public class PixelMapJSON
{
    public int width;
    public int height;
    public List<PixelData> pixels = new List<PixelData>();
}

public class PixelEditorWindow : EditorWindow
{
    //Size
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;

    //Paint
    private int[,] _grid;
    private Vector2 _scroll;

    private Vector2 _mainScroll;

    private List<ObjectPixelData> _objectList;
    private ObjectPixelData _selected;
    private string _objectName = "NewObject";
    [SerializeField] private Material _sharedMaterial;

    //Color
    private Color _selectedColor = Color.white;
    private Color[,] _colorGrid;

    [MenuItem("Tools/Pixel Editor")]
    public static void Open()
    {
        GetWindow<PixelEditorWindow>();
    }
    private void OnEnable()
    {
        LoadObjectList();

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        this._grid = new int[this._width, this._height];

        for (int x = 0; x < this._width; x++)
        {
            for (int z = 0; z < this._height; z++)
            {
                this._grid[x, z] = -1;
            }
        }
        InitGrid();
    }
    private void OnGUI()
    {
        _mainScroll = EditorGUILayout.BeginScrollView(_mainScroll);

        GUILayout.Space(5);
        GUILayout.Label("Material Settings", EditorStyles.boldLabel);

        _sharedMaterial = (Material)EditorGUILayout.ObjectField(
            "Pixel Material",
            _sharedMaterial,
            typeof(Material),
            false
        );

        if (_sharedMaterial == null)
        {
            EditorGUILayout.HelpBox("chưa gán Material (URP/Lit)!", MessageType.Warning);
        }

        DrawGridSettings();

        DrawPalette();

        GUILayout.Space(5);
        GUILayout.Label("Color Picker", EditorStyles.boldLabel);

        _selectedColor = EditorGUILayout.ColorField("Selected Color", _selectedColor);

        DrawGrid();

        GUILayout.Space(10);

        if (GUILayout.Button("Save Prefab"))
        {
            SavePrefab();
        }

        EditorGUILayout.EndScrollView();
    }
    private void LoadObjectList()
    {
        this._objectList = new List<ObjectPixelData>();

        string[] guids = AssetDatabase.FindAssets("t:ObjectPixelData");

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<ObjectPixelData>(path);

            if (obj != null)
                this._objectList.Add(obj);
        }
    }
    private void DrawGridSettings()
    {
        GUILayout.Label("Grid Setting", EditorStyles.boldLabel);

        int newWidth = EditorGUILayout.IntField("Width", this._width);
        int newHeight = EditorGUILayout.IntField("height", this._height);

        newWidth = Mathf.Max(1, newWidth);
        newHeight = Mathf.Max(1, newHeight);

        if (newWidth != _width || newHeight != _height)
        {
            _width = newWidth;
            _height = newHeight;
            InitGrid();
        }

        if (GUILayout.Button("Reset Grid"))
        {
            InitGrid();
        }
    }
    private void InitGrid()
    {
        _grid = new int[_width, _height];
        _colorGrid = new Color[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _grid[x, y] = -1;
                _colorGrid[x, y] = Color.clear;
            }
        }
    }
    private void DrawPalette()
    {
        GUILayout.Label("Palette", EditorStyles.boldLabel);

        this._scroll = GUILayout.BeginScrollView(this._scroll, GUILayout.Height(100));

        GUILayout.BeginHorizontal();

        foreach (var obj in this._objectList)
        {
            GUI.backgroundColor = (this._selected == obj) ? Color.green : Color.white;

            Texture tex = obj.icon != null ? obj.icon.texture : Texture2D.grayTexture;

            if (GUILayout.Button(tex, GUILayout.Width(60), GUILayout.Height(60)))
            {
                this._selected = obj;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        GUI.backgroundColor = Color.white;
    }
    private void DrawGrid()
    {
        GUILayout.Space(10);
        GUILayout.Label("Grid", EditorStyles.boldLabel);

        for (int y = 0; y < this._height; y++)
        {
            GUILayout.BeginHorizontal();

            for (int x = 0; x < this._width; x++)
            {
                DrawCell(x, y);
            }
            GUILayout.EndHorizontal();
        }
    }
    //Xử lý từng ô để tạo hình prefab
    private void DrawCell(int x, int y)
    {
        Rect rect = GUILayoutUtility.GetRect(30, 30);

        Color cellColor = _colorGrid[x, y];
        Color drawColor = cellColor == Color.clear ? new Color(0.2f, 0.2f, 0.2f) : cellColor;

        EditorGUI.DrawRect(rect, drawColor);

        if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 1)
            {
                _colorGrid[x, y] = Color.clear;
            }
            else
            {
                _colorGrid[x, y] = _selectedColor;
            }

            Repaint();
        }
    }
    //Lưu
    private void SavePrefab()
    {
        PixelMapJSON mapJSON = new PixelMapJSON();
        mapJSON.width = _width;
        mapJSON.height = _height;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Color color = _colorGrid[x, y];
                if (color == Color.clear) continue;

                PixelData pd = new PixelData();
                pd.x = x;
                pd.y = y;
                pd.z = 0;
                pd.r = color.r;
                pd.g = color.g;
                pd.b = color.b;
                pd.a = color.a;

                mapJSON.pixels.Add(pd);
            }
        }

        // Chuyển thành JSON
        string json = JsonUtility.ToJson(mapJSON, true);
        System.IO.File.WriteAllText($"Assets/Prefabs/{_objectName}.json", json);
        AssetDatabase.Refresh();
    }
}

