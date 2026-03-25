using UnityEngine;

public class TowerPlatform : MonoBehaviour
{
    public bool isOccupied;
    private TowerBase _currentTower;

    [SerializeField] private Transform towerPoint;

    public void PlaceTower(GameObject prefab)
    {
        if (isOccupied) return;

        Vector3 pos = towerPoint != null
            ? towerPoint.position
            : transform.position + Vector3.up * 1f;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        this._currentTower = go.GetComponent<TowerBase>();

        isOccupied = true;
    }

    private void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}