using UnityEngine;

public class TowerPlatform : MonoBehaviour
{
    private TowerBase _currentTower;

    [SerializeField] private Transform towerPoint;

    [Header("Visual")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.green;

    private bool _isHighlighting = false;

    public bool IsOccupied => _currentTower != null;

    public void PlaceTower(GameObject prefab)
    {
        if (IsOccupied) return;

        Vector3 pos = towerPoint != null ? towerPoint.position : transform.position + Vector3.up * 1f;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        go.transform.rotation = Quaternion.Euler(90f, 0, 0);
        _currentTower = go.GetComponent<TowerBase>();

        SetHighlight(false);
    }

    public void SetHighlight(bool value)
    {
        _isHighlighting = value;

        if (!value)
        {
            transform.localScale = Vector3.one;

            if (_renderer != null)
                _renderer.material.color = normalColor;
        }
    }

    private void Update()
    {
        if (!_isHighlighting || _renderer == null) return;

        float scale = 1f + Mathf.Sin(Time.time * 5f) * 0.1f;
        transform.localScale = new Vector3(scale, 1f, scale);

        float t = (Mathf.Sin(Time.time * 5f) + 1f) / 2f;
        _renderer.material.color = Color.Lerp(normalColor, highlightColor, t);
    }
}