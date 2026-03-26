using UnityEngine;
using UnityEngine.UI;

public class SoundPanelUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panelGroup;

    [Header("Sliders")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Buttons")]
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    private void Start()
    {
        HidePanel();

        _musicSlider.value = AudioManager.Instance.musicVolume;
        _sfxSlider.value = AudioManager.Instance.sfxVolume;

        _openButton.onClick.AddListener(ShowPanel);
        _closeButton.onClick.AddListener(HidePanel);

        _musicSlider.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.SetMusicVolume(value);
        });

        _sfxSlider.onValueChanged.AddListener((value) =>
        {
            AudioManager.Instance.SetSFXVolume(value);
        });
    }

    private void ShowPanel()
    {
        _panelGroup.alpha = 1f;
        _panelGroup.interactable = true;
        _panelGroup.blocksRaycasts = true;
    }

    private void HidePanel()
    {
        _panelGroup.alpha = 0f;
        _panelGroup.interactable = false;
        _panelGroup.blocksRaycasts = false;
    }
}