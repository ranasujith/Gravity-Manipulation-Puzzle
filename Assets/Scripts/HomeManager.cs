using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Header("UI BUTTONS")]
    public Button playBtn;
    public Button quitBtn;

    [Header("SETTINGS PAGE")]
    public Button settingBtn;
    public GameObject settingsPage;
    public Button closeSettings;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("AUDIO CLIPS")]
    public AudioClip bgAudio;
    public AudioClip iconClickSFX;
    private void Start()
    {
        AudioManager.Instance.PlayMusic(bgAudio);
        playBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(iconClickSFX);
            DOTween.KillAll();
            SceneManager.LoadScene("GameScene");
        });

        quitBtn.onClick.AddListener(QuitGame);
        settingBtn.onClick.AddListener(OpenSettings);
        closeSettings.onClick.AddListener(CloseSettings);

        AddButtonEffects(playBtn);
        AddButtonEffects(quitBtn);

        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
        GameSettingsHandler.Instance.OnSettingsChanged += RefreshSettingUI;
        RefreshSettingUI();
    }

    private void OnDisable()
    {
        GameSettingsHandler.Instance.OnSettingsChanged -= RefreshSettingUI;
    }

    void RefreshSettingUI()
    {
        musicToggle.SetIsOnWithoutNotify(GameSettingsHandler.Instance.IsMusicOn);
        sfxToggle.SetIsOnWithoutNotify(GameSettingsHandler.Instance.IsSFXOn);

        musicToggle.GetComponent<ChangeImage>()
            .SetImageOn(GameSettingsHandler.Instance.IsMusicOn);

        sfxToggle.GetComponent<ChangeImage>()
            .SetImageOn(GameSettingsHandler.Instance.IsSFXOn);
    }
    void OnMusicToggleChanged(bool isOn)
    {
        GameSettingsHandler.Instance.MusicEnable(isOn);
        musicToggle.GetComponent<ChangeImage>().SetImageOn(isOn);
    }

    void OnSFXToggleChanged(bool isOn)
    {
        GameSettingsHandler.Instance.SFXEnable(isOn);
        sfxToggle.GetComponent<ChangeImage>().SetImageOn(isOn);
    }

    void AddButtonEffects(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();

        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) =>
        {
            button.transform.DOScale(1.1f, 0.2f);
        });
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) =>
        {
            button.transform.DOScale(1f, 0.2f);
        });
        trigger.triggers.Add(exit);
    }
    void OpenSettings()
    {
        settingsPage.SetActive(true);
        closeSettings.GetComponent<Image>().raycastTarget = true;
        AudioManager.Instance.PlaySFX(iconClickSFX);

        settingsPage.transform.localScale = Vector3.one * 0.8f;
    }
    void CloseSettings()
    {
        AudioManager.Instance.PlaySFX(iconClickSFX);
        closeSettings.GetComponent<Image>().raycastTarget = false;
        settingsPage.transform.DOScale(0.8f, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                settingsPage.SetActive(false);
            });
    }

    void QuitGame()
    {
        AudioManager.Instance.PlaySFX(iconClickSFX);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
