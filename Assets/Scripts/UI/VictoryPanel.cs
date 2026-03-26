using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _nextButton;

    private void Awake()
    {
        Hide();

        _replayButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Hide();
        });

        _nextButton.onClick.AddListener(() =>
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("No more scenes in build settings!");
            }
            Hide();
        });
    }

    private void Start()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        _nextButton.interactable = nextSceneIndex < SceneManager.sceneCountInBuildSettings;
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