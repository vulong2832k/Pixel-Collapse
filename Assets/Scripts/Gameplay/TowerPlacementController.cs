using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public static TowerPlacementController Instance;

    [SerializeField] private TowerPlatform[] _platforms;

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

    private void Update()
    {
        AddTower();
    }
    private void AddTower()
    {
        if (!_isPlacing) return;

        Vector2 touchPos = Vector2.zero;
        bool touchDetected = false;

        // Mobile touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchPos = touch.position;
                touchDetected = true;
            }
        }

        // PC mouse
        if (Input.GetMouseButtonDown(0))
        {
            touchPos = Input.mousePosition;
            touchDetected = true;
        }

        if (!touchDetected) return;

        Ray ray = _cam.ScreenPointToRay(touchPos);

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
    public void StartPlacing(GameObject prefab, int count)
    {
        _selectedTower = prefab;
        _isPlacing = true;
        _remainingPlaceCount = count;

        UpdatePlatformHighlight(true);
    }
    private void UpdatePlatformHighlight(bool value)
    {
        foreach (var platform in _platforms)
        {
            if (platform != null && !platform.IsOccupied)
            {
                platform.SetHighlight(value);
            }
        }
    }
}