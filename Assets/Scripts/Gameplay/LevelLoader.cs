using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public int levelNumber = 1;

    private LevelData _levelData;

    public TowerDatabase towerDatabase;

    void Start()
    {
        LoadLevel();

        if (_levelData == null)
        {
            return;
        }

        LoadMap(_levelData.levelNumber);
        SpawnTowers();
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
}