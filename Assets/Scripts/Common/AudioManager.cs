using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (GameSettingsHandler.Instance != null)
            GameSettingsHandler.Instance.OnSettingsChanged += ApplySettings;
    }

    private void OnDisable()
    {
        if (GameSettingsHandler.Instance != null)
            GameSettingsHandler.Instance.OnSettingsChanged -= ApplySettings;
    }

    private void Start()
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        if (GameSettingsHandler.Instance == null) return;

        SetMusicEnabled(GameSettingsHandler.Instance.IsMusicOn);
        SetSFXEnabled(GameSettingsHandler.Instance.IsSFXOn);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;

        if (GameSettingsHandler.Instance.IsMusicOn)
            musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicEnabled(bool enable)
    {
        musicSource.mute = !enable;

        if (enable && !musicSource.isPlaying && musicSource.clip != null)
            musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        if (!GameSettingsHandler.Instance.IsSFXOn)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void SetSFXEnabled(bool enable)
    {
        sfxSource.mute = !enable;
    }
}
