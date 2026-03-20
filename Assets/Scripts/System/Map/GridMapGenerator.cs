using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridMapGenerator : MonoBehaviour
{
    public enum EditMode
    {
        None,
        PaintGround,
        BuildWall,
        Delete,
    }

    [Header("Grid Size:")]
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _cellSize;

    [Header("Mode:")]
    private EditMode _editMode;

    [Header("References:")]
    [SerializeField] private Transform _mapRoot;
    [SerializeField] private GameObject[] _groundPrefabs;
    [SerializeField] private GameObject[] _wallPrefabs;

    [SerializeField] private MapData _mapData;

    [Header("Save Settings")]
    public string Map = "Map_";

    [Header("Selected:")]
    [HideInInspector] public int SelectedGround = 0;
    [HideInInspector] public int SelectedWall = 0;

    [HideInInspector] public string CurrentMapPath;

    private int[,] _runtimeMapData;

    private GridCell[,] _cells;

    private List<GameObject>[,] _wallStacks;

    public int Width
    {
        get => _width;
        set => _width = value;
    }

    public int Height
    {
        get => _height;
        set => _height = value;
    }
    public float CellSize => _cellSize;
    public Transform MapRoot => _mapRoot;
    public GameObject[] GroundPrefabs => _groundPrefabs;
    public GameObject[] WallPrefabs => _wallPrefabs;
    public MapData MapData => _mapData;

    public EditMode Mode
    {
        get => _editMode;
        set => _editMode = value;
    }

    public void InitRuntimeData()
    {
        _runtimeMapData = new int[_width, _height];
        _cells = new GridCell[_width, _height];
        _wallStacks = new List<GameObject>[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                _wallStacks[x, z] = new List<GameObject>();
            }
        }
    }
    public bool IsInitialized()
    {
        return _cells != null && _runtimeMapData != null;
    }
    public bool CellsSizeMismatch()
    {
        return _cells == null || _cells.GetLength(0) != _width || _cells.GetLength(1) != _height;
    }
    //----------------------------------------------------------------------------------------
    public void RegisterCell(int x, int z, GridCell cell)
    {
        this._cells[x, z] = cell;
    }
    public GridCell GetCell(int x, int z)
    {
        return this._cells[x, z];
    }
    public void SetCellData(int x, int z, int type)
    {
        this._runtimeMapData[x, z] = type;
    }
    public int GetCellData(int x, int z)
    {
        return this._runtimeMapData[x, z];
    }
    //----------------------------------------------------------------------------------------
    public void AddWall(int x, int z)
    {
        if (_wallStacks == null)
            InitRuntimeData();

        // CHECK BOUNDS
        if (x < 0 || x >= _width || z < 0 || z >= _height)
        {
            Debug.LogWarning("Sai vị trí!");
            return;
        }

        // CHECK WALL PREFAB
        if (_wallPrefabs == null || _wallPrefabs.Length == 0)
        {
            Debug.LogWarning("Chưa có Wall được chỉ định!");
            return;
        }

        // CHECK SELECTED WALL
        if (SelectedWall < 0 || SelectedWall >= _wallPrefabs.Length)
        {
            Debug.LogWarning("Chưa chọn Wall!");
            return;
        }

        var stack = _wallStacks[x, z];

        int height = 1;

        while (true)
        {
            bool found = false;

            foreach (var w in stack)
            {
                int wallHeight = Mathf.RoundToInt(w.transform.position.y / _cellSize);

                if (wallHeight == height)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                break;

            height++;
        }

        Vector3 pos = new Vector3(x * _cellSize, height * _cellSize, z * _cellSize);

        GameObject wall = Instantiate(_wallPrefabs[SelectedWall], pos, Quaternion.identity, _mapRoot);

        Undo.RegisterCreatedObjectUndo(wall, "Add Wall");

        wall.name = $"Wall_{x}_{z}_{height}_{SelectedWall}";

        stack.Add(wall);
    }
    public void RemoveWall(int x, int z)
    {
        if (_wallStacks == null)
        {
            InitRuntimeData();
        }

        var stack = this._wallStacks[x, z];

        if (stack.Count == 0) return;

        GameObject top = stack[stack.Count - 1];

        stack.RemoveAt(stack.Count - 1);

        Undo.DestroyObjectImmediate(top);
    }
    public void RemoveWallByObject(GameObject wall)
    {
        if(_wallStacks == null)
            InitRuntimeData();

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                var stack = _wallStacks[x, z];

                if (stack.Contains(wall))
                {
                    stack.Remove(wall);
                    Undo.DestroyObjectImmediate(wall);
                    return;
                }
            }
        }
        Undo.DestroyObjectImmediate(wall);
    }
    public List<GameObject> GetWallStack(int x, int z)
    {
        if (_wallStacks == null)
            return null;
        if (x < 0 || x >= _width || z < 0 || z >= _height)
            return null;

        return _wallStacks[x, z];
    }
}