using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMapGenerator))]
public class GridMapGeneratorEditor : Editor
{
    //Ghost Block
    Vector3 previewPos;
    bool hasPreview = false;

    //Ghost Object
    GameObject previewObject;

    string[] mapFiles;
    int selectedMapIndex = -1;

    private void OnSceneGUI()
    {
        if (Tools.viewToolActive)
        {
            return;
        }

        if (Event.current.type == EventType.MouseMove)
        {
            SceneView.RepaintAll();
        }
        GridMapGenerator gen = (GridMapGenerator)target;

        if (!gen.IsInitialized() || gen.CellsSizeMismatch())
        {
            gen.InitRuntimeData();
            RebuildDataFromScene(gen);
        }

        if (gen.MapRoot == null) return;

        Event e = Event.current;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCell cell = hit.collider.GetComponent<GridCell>();

            if (cell != null)
            {
                Vector3 p = hit.collider.transform.position + hit.normal * gen.CellSize;

                previewPos = new Vector3(
                    Mathf.RoundToInt(p.x / gen.CellSize) * gen.CellSize,
                    Mathf.RoundToInt(p.y / gen.CellSize) * gen.CellSize,
                    Mathf.RoundToInt(p.z / gen.CellSize) * gen.CellSize);

                hasPreview = true;
            }
            else
            {
                hasPreview = false;
            }
            // BUILD WALL MODE
            if (gen.Mode == GridMapGenerator.EditMode.BuildWall)
            {
                // LEFT CLICK: ADD WALL
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Vector3 spawnPos = hit.point;

                    spawnPos.x = Mathf.Round(spawnPos.x / gen.CellSize) * gen.CellSize;
                    spawnPos.z = Mathf.Round(spawnPos.z / gen.CellSize) * gen.CellSize;
                    spawnPos.y = 0;

                    int x = Mathf.RoundToInt(spawnPos.x / gen.CellSize);
                    int z = Mathf.RoundToInt(spawnPos.z / gen.CellSize);

                    Undo.RegisterCompleteObjectUndo(gen, "Add Wall");
                    gen.AddWall(x, z);
                    e.Use();
                }

                // RIGHT CLICK: REMOVE WALL
                if (e.type == EventType.MouseDown && e.button == 1 && !Tools.viewToolActive)
                {
                    GameObject obj = hit.collider.gameObject;

                    gen.RemoveWallByObject(obj);
                    e.Use();
                }
            }

            // PAINT GROUND MODE
            if (gen.Mode == GridMapGenerator.EditMode.PaintGround)
            {
                int x = Mathf.RoundToInt(previewPos.x / gen.CellSize);
                int z = Mathf.RoundToInt(previewPos.z / gen.CellSize);

                if (x < 0 || x >= gen.Width || z < 0 || z >= gen.Height)
                    return;

                //LEFT CLICK: PAINT
                if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
                {
                    GridCell gridCell = gen.GetCell(x, z);

                    if (gridCell != null)
                    {
                        ReplaceCell(gen, x, z, gen.SelectedGround);
                    }
                    else
                    {
                        CreateCell(gen, x, z, gen.SelectedGround);
                    }
                        e.Use();
                }
                //RIGHT CLICK: DELETE
                if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1)
                {
                    GridCell gridCell = gen.GetCell(x, z);

                    if (gridCell != null)
                    {
                        DeleteCell(gen, x, z);
                    }

                    e.Use();
                }
            }
            
        }

        if (hasPreview)
        {
            Handles.DrawWireCube(previewPos, Vector3.one * gen.CellSize);

            Handles.color = new Color(0, 1, 0, 0.1f);
            Handles.CubeHandleCap(0, previewPos, Quaternion.identity, gen.CellSize, EventType.Repaint);
        }
    }
    public override void OnInspectorGUI()
    {
        GridMapGenerator generator = (GridMapGenerator)target;

        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("=== EDIT MODE ===", EditorStyles.boldLabel);

        generator.Mode = (GridMapGenerator.EditMode)EditorGUILayout.EnumPopup(
            "Edit Mode",
            generator.Mode
        );

        GUILayout.Space(10);

        DrawGroundSelector(generator);

        GUILayout.Space(10);

        DrawWallSelector(generator);

        GUILayout.Space(10);

        GUILayout.Label("=== MAP TOOLS ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Create New Map"))
        {
            CreateNewMap(generator);
        }

        if (GUILayout.Button("Generate Grid"))
        {
            Generate(generator);
        }

        if (GUILayout.Button("Clear Grid"))
        {
            Clear(generator);
        }

        if (GUILayout.Button("Save Map"))
        {
            Save(generator);
        }

        GUILayout.Space(20);

        GUILayout.Label("=== MAP LIST ===", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Map List"))
        {
            RefreshMapList(generator);
        }
        if (mapFiles != null && mapFiles.Length > 0)
        {
            string[] names = new string[mapFiles.Length];

            for (int i = 0; i < mapFiles.Length; i++)
            {
                names[i] = System.IO.Path.GetFileName(mapFiles[i]);
            }
            selectedMapIndex = EditorGUILayout.Popup("Select Map", selectedMapIndex, names);

            if (GUILayout.Button("Load Map"))
            {
                if (selectedMapIndex >= 0 && selectedMapIndex < mapFiles.Length)
                {
                    LoadMap(generator, mapFiles[selectedMapIndex]);
                }
            }
        }
    }
    private void LoadMap(GridMapGenerator gen, string path)
    {
        Clear(gen);

        MapData map = MapSerializer.LoadMap(path);

        gen.Width = map.width;
        gen.Height = map.height;

        gen.InitRuntimeData();

        for (int x = 0; x < gen.Width; x++)
        {
            for (int z = 0; z < gen.Height; z++)
            {
                int index = x + z * gen.Width;
                int type = map.cellTypes[index];

                if (type >= 0)
                {
                    CreateCell(gen, x, z, type);
                }
            }
        }

        foreach (var wall in map.walls)
        {
            Vector3 pos = new Vector3(wall.x * gen.CellSize, wall.height * gen.CellSize, wall.z * gen.CellSize);

            GameObject wallObj = (GameObject)PrefabUtility.InstantiatePrefab(gen.WallPrefabs[wall.type]);

            wallObj.transform.position = pos;
            wallObj.transform.SetParent(gen.MapRoot);

            wallObj.name = $"Wall_{wall.x}_{wall.z}_{wall.height}_{wall.type}";

            if (gen.GetWallStack(wall.x, wall.z) != null)
            {
                gen.GetWallStack(wall.x, wall.z).Add(wallObj);
            }
        }

        gen.CurrentMapPath = path;

        Debug.Log("Map Loaded!");
    }
    private void DrawGroundSelector(GridMapGenerator generator)
    {
        if (generator.GroundPrefabs == null || generator.GroundPrefabs.Length == 0) return;

        GUILayout.Label("Ground Tiles", EditorStyles.boldLabel);

        int columns = 4;

        GUIContent[] contents = new GUIContent[generator.GroundPrefabs.Length];

        for (int i = 0; i < contents.Length; i++)
        {
            var prefab = generator.GroundPrefabs[i];

            Texture preview = AssetPreview.GetAssetPreview(prefab);

            contents[i] = new GUIContent(preview, prefab.name);
        }
        generator.SelectedGround = GUILayout.SelectionGrid(generator.SelectedGround, contents, columns, GUILayout.Height(80));
    }

    private void DrawWallSelector(GridMapGenerator generator)
    {
        if (generator.WallPrefabs == null || generator.WallPrefabs.Length == 0) return;

        GUILayout.Label("Wall Tiles", EditorStyles.boldLabel);

        int columns = 4;

        GUIContent[] contents = new GUIContent[generator.WallPrefabs.Length];

        for (int i = 0; i < contents.Length; i++)
        {
            var prefab = generator.WallPrefabs[i];

            Texture preview = AssetPreview.GetAssetPreview(prefab);

            contents[i] = new GUIContent(preview, prefab.name);
        }
        generator.SelectedWall = GUILayout.SelectionGrid(generator.SelectedWall, contents, columns, GUILayout.Height(80));
    }
    private void Generate(GridMapGenerator gen)
    {
        Clear(gen);

        gen.InitRuntimeData();

        for (int x = 0; x < gen.Width; x++)
        {
            for (int z = 0; z < gen.Height; z++)
            {
                CreateCell(gen, x, z, 0);
            }
        }
    }

    private void CreateCell(GridMapGenerator gen, int x, int z, int type)
    {
        Vector3 pos = new Vector3(x * gen.CellSize, 0, z * gen.CellSize);

        GameObject cellObj = (GameObject)PrefabUtility.InstantiatePrefab(
            gen.GroundPrefabs[type]
        );

        cellObj.transform.position = pos;
        cellObj.transform.SetParent(gen.MapRoot);

        GridCell cell = cellObj.GetComponent<GridCell>();

        if (cell == null)
            cell = cellObj.AddComponent<GridCell>();

        cell.x = x;
        cell.z = z;
        cell.typeIndex = type;

        cellObj.name = $"Cell_{x}_{z}";

        gen.RegisterCell(x, z, cell);
        gen.SetCellData(x, z, type);
    }

    private void ReplaceCell(GridMapGenerator gen, int x, int z, int newType)
    {
        GridCell oldCell = gen.GetCell(x, z);

        if (oldCell == null) return;

        Vector3 pos = oldCell.transform.position;

        Undo.DestroyObjectImmediate(oldCell.gameObject);

        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(gen.GroundPrefabs[newType]);

        Undo.RegisterCreatedObjectUndo(newObj, "Paint Cell");

        newObj.transform.position = pos;
        newObj.transform.SetParent(gen.MapRoot);

        GridCell newCell = newObj.GetComponent<GridCell>();

        if (newCell == null)
            newCell = newObj.AddComponent<GridCell>();

        newCell.x = x;
        newCell.z = z;
        newCell.typeIndex = newType;

        newObj.name = $"Cell_{x}_{z}";

        gen.RegisterCell(x, z, newCell);

        gen.SetCellData(x, z, newType);
    }
    private void DeleteCell(GridMapGenerator gen, int x, int z)
    {
        GridCell cell = gen.GetCell(x, z);

        if (cell == null) return;

        Undo.DestroyObjectImmediate(cell.gameObject);

        gen.RegisterCell(x, z, null);
        gen.SetCellData(x, z, -1);
    }
    private void RefreshMapList(GridMapGenerator gen)
    {
        string folder = UnityEngine.Application.dataPath + $"/Resources/Gameplay/Maps/";

        if (!System.IO.Directory.Exists(folder))
            System.IO.Directory.CreateDirectory(folder);

        mapFiles = System.IO.Directory.GetFiles(folder, "*.json",
            System.IO.SearchOption.AllDirectories);

        for (int i = 0; i < mapFiles.Length; i++)
            mapFiles[i] = mapFiles[i].Replace("\\", "/");
    }
    private void Clear(GridMapGenerator gen)
    {
        if (gen.MapRoot == null) return;

        while (gen.MapRoot.childCount > 0)
        {
            Undo.DestroyObjectImmediate(gen.MapRoot.GetChild(0).gameObject);
        }

        gen.InitRuntimeData();
    }
    private void CreateNewMap(GridMapGenerator gen)
    {
        Clear(gen);

        gen.CurrentMapPath = null;

        gen.InitRuntimeData();

        for (int x = 0; x < gen.Width; x++)
        {
            for (int z = 0; z < gen.Height; z++)
            {
                CreateCell(gen, x, z, 0);
            }
        }

        Debug.Log("New Map Created!");
    }
    private void Save(GridMapGenerator gen)
    {
        MapData mapData = new MapData();

        mapData.width = gen.Width;
        mapData.height = gen.Height;

        mapData.cellTypes = new int[gen.Width * gen.Height];

        for (int x = 0; x < gen.Width; x++)
        {
            for (int z = 0; z < gen.Height; z++)
            {
                int index = x + z * gen.Width;
                mapData.cellTypes[index] = gen.GetCellData(x, z);
            }
        }
        //Wall
        mapData.walls.Clear();

        foreach (Transform child in gen.MapRoot)
        {
            if (child.name.StartsWith("Wall_"))
            {
                string[] parts = child.name.Split('_');

                WallData wall = new WallData();

                wall.x = int.Parse(parts[1]);
                wall.z = int.Parse(parts[2]);
                wall.height = int.Parse(parts[3]);
                wall.type = int.Parse(parts[4]);

                mapData.walls.Add(wall);
            }
        }

        string map = gen.Map;

        string folder = UnityEngine.Application.dataPath + $"/Resources/Gameplay/Maps/";

        if(!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string fileName = $"{map}.json";

        string path = folder + fileName;

        MapSerializer.SaveMap(mapData, path);

        gen.CurrentMapPath = path;

        Debug.Log("Map Saved: " + path);
    }
    private void RebuildDataFromScene(GridMapGenerator gen)
    {
        foreach (Transform child in gen.MapRoot)
        {
            if (child.name.StartsWith("Cell_"))
            {
                string[] parts = child.name.Split('_');

                int x = int.Parse((string)parts[1]);
                int z = int.Parse((string)parts[2]);

                GridCell cell = child.GetComponent<GridCell>();

                gen.RegisterCell(x, z, cell);
                gen.SetCellData(x, z, cell.typeIndex);
            }
            else if (child.name.StartsWith("Wall_"))
            {
                string[] parts = child.name.Split('_');

                int x = int.Parse((string)parts[1]);
                int z = int.Parse((string)parts[2]);

                if (gen.GetWallStack(x, z) != null)
                {
                    gen.GetWallStack(x, z).Add(child.gameObject);
                }
            }
        }
    }
}