using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public MapDataBase dataBase;

    public GameObject[] groundPrefabs;
    public GameObject[] wallPrefabs;

    public Transform mapRoot;

    public void LoadMap(int index)
    {
        MapData map = dataBase.LoadMap(index);

        if (map == null) return;

        float cellSize = 1f;

        for (int x = 0; x < map.width; x++)
        {
            for (int z = 0; z < map.height; z++)
            {
                int type = map.cellTypes[x + z * map.width];

                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);

                Instantiate(groundPrefabs[type], pos, Quaternion.identity, mapRoot);
            }
        }
        foreach (var wall in map.walls)
        {
            Vector3 pos = new Vector3(wall.x * cellSize, wall.height * cellSize, wall.z * cellSize);

            Instantiate(wallPrefabs[wall.type], pos, Quaternion.identity, mapRoot);
        }
    }
}
