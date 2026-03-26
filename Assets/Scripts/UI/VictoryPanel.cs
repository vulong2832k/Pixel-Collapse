using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _nextButton;

    private LevelLoader _levelLoader;

    private void Awake()
    {
        _levelLoader = FindObjectOfType<LevelLoader>();
        Hide();

        _replayButton.onClick.AddListener(() =>
        {
            if (_levelLoader != null)
                _levelLoader.ReloadCurrentLevel();
            Hide();
        });

        _nextButton.onClick.AddListener(() =>
        {
            if (_levelLoader != null)
            {
                int currentIndex = SceneManager.GetActiveScene().buildIndex;
                int totalLevels = SceneManager.sceneCountInBuildSettings;

                if (currentIndex < totalLevels - 1)
                {
                    _levelLoader.LoadNextLevel();
                }
                else
                {
                    Debug.Log("Hết level rồi bro!");
                }
            }

            Hide();
        });
    }
    private void Start()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalLevels = SceneManager.sceneCountInBuildSettings;

        if (currentIndex >= totalLevels - 1)
        {
            _nextButton.interactable = false;
        }
    }
    public void Show()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
