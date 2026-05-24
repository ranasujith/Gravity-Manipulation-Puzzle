using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsHandler : MonoBehaviour
{
    public static GameSettingsHandler Instance { get; private set; }

    private const string KEY_MUSIC = "SETTING_MUSIC";
    private const string KEY_SFX = "SETTING_SFX";

    public bool IsMusicOn { get; private set; }
    public bool IsSFXOn { get; private set; }

    public event System.Action OnSettingsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();

        SceneManager.sceneLoaded += OnSceneLoaded;

        ApplyAllSettings();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAllSettings();
    }

    private void LoadSettings()
    {
        IsMusicOn = PlayerPrefs.GetInt(KEY_MUSIC, 1) == 1;
        IsSFXOn = PlayerPrefs.GetInt(KEY_SFX, 1) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(KEY_MUSIC, IsMusicOn ? 1 : 0);
        PlayerPrefs.SetInt(KEY_SFX, IsSFXOn ? 1 : 0);

        PlayerPrefs.Save();
    }

    private void ApplyAllSettings()
    {
        ApplyMusic(IsMusicOn);
        ApplySFX(IsSFXOn);
    }

    private void ApplyMusic(bool enable)
    {
        AudioManager.Instance?.SetMusicEnabled(enable);
    }

    private void ApplySFX(bool enable)
    {
        AudioManager.Instance?.SetSFXEnabled(enable);
    }


    public void MusicEnable(bool enable)
    {
        IsMusicOn = enable;
        ApplyMusic(enable);
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    public void SFXEnable(bool enable)
    {
        IsSFXOn = enable;
        ApplySFX(enable);
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }
}