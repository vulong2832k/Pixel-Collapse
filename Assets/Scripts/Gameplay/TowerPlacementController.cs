using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public static TowerPlacementController Instance;

    private GameObject _selectedTower;
    private bool _isPlacing;
    private int _remainingPlaceCount;

    private Camera _cam;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _cam = Camera.main;
    }

    public void StartPlacing(GameObject prefab, int count)
    {
        _selectedTower = prefab;
        _isPlacing = true;
        _remainingPlaceCount = count;
    }

    private void Update()
    {
        AddTower();
    }
    private void AddTower()
    {
        if (!_isPlacing) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TowerPlatform platform = hit.collider.GetComponent<TowerPlatform>();

                if (platform != null && !platform.IsOccupied)
                {
                    platform.PlaceTower(_selectedTower);

                    _remainingPlaceCount--;

                    if (_remainingPlaceCount <= 0)
                    {
                        _isPlacing = false;
                    }
                }
            }
        }
    }
}