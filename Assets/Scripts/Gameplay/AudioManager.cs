using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = value;
        Save();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = value;
        Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("MUSIC_VOLUME", musicVolume);
        PlayerPrefs.SetFloat("SFX_VOLUME", sfxVolume);
    }

    private void Load()
    {
        musicVolume = PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }
}