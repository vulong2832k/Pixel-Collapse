using UnityEngine;

public class TowerPlatform : MonoBehaviour
{
    private TowerBase _currentTower;

    [SerializeField] private Transform towerPoint;

    public bool IsOccupied => _currentTower != null;

    public void PlaceTower(GameObject prefab)
    {
        if (IsOccupied) return;

        Vector3 pos = towerPoint != null ? towerPoint.position : transform.position + Vector3.up * 1f;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        go.transform.rotation = Quaternion.Euler(90f, 0, 0);
        _currentTower = go.GetComponent<TowerBase>();
    }
}