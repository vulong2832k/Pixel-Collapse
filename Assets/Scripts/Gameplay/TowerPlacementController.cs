using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public static TowerPlacementController Instance;

    private GameObject _selectedTower;
    private bool _isPlacing;
    private Camera _cam;

    private void Awake()
    {
        Instance = this;
        _cam = Camera.main;
    }

    public void StartPlacing(GameObject prefab)
    {
        _selectedTower = prefab;
        _isPlacing = true;
    }

    private void Update()
    {
        if (!_isPlacing) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TowerPlatform platform = hit.collider.GetComponent<TowerPlatform>();

                if (platform != null && !platform.isOccupied)
                {
                    platform.PlaceTower(_selectedTower);
                    _isPlacing = false;
                }
            }
        }
    }
}