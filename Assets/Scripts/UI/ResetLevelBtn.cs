using UnityEngine;
using UnityEngine.UI;

public class ResetLevelBtn : MonoBehaviour
{
    [SerializeField] private Button _button;
    private LevelLoader _levelLoader;

    private void Awake()
    {
        _levelLoader = FindObjectOfType<LevelLoader>();

        _button.onClick.AddListener(() =>
        {
            if (_levelLoader != null)
            {
                _levelLoader.ReloadCurrentLevel();
            }
        });
    }
}
