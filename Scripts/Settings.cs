using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.Localization.Settings;

public class Settings : MonoBehaviour
{
    [SerializeField] private Color backgroundActiveColor;
    [SerializeField] private Color handleActiveColor;
    [SerializeField] private Toggle musicSwitch;
    [SerializeField] private Toggle soundSwitch;

    [SerializeField] private GameObject MusicAndSound;
    [SerializeField] private GameObject Localization;
    [SerializeField] private GameObject LanguagesPanel;
    [SerializeField] private Transform Content;
    [SerializeField] private Button OpenLanguagesPanelButton;
    [SerializeField] private Button CloseLanguagesPanelButton;
    [SerializeField] private Button RestorePurchasesButton;

    private bool active = false;
    private RectTransform musicHandle, soundHandle;
    private Image musicBackgroundImage, musicHandleImage, soundBackgroundImage, soundHandleImage;
    private Color backgroundDefaultColor, handleDefaultColor;
    private Vector2 musicHandlePosition, soundHandlePosition;


    private void Awake ()
    {
        StartCoroutine(SetUI());

        if (OpenLanguagesPanelButton != null)
        {
            OpenLanguagesPanelButton.onClick.AddListener(OpenLanguagesPanel);
        }

        if (CloseLanguagesPanelButton != null)
        {
            CloseLanguagesPanelButton.onClick.AddListener(CloseLanguagesPanel);
        }

        if (RestorePurchasesButton != null)
        {
            RestorePurchasesButton.onClick.AddListener(RestorePurchases);
        }
    }

    private void OnEnable()
    {
        LanguagesPanel.SetActive(false);
        LanguagesPanel.transform.localScale = Vector3.zero;
    }

    private void EnableMusic (bool enabled)
    {
        Sound.MusicEnabled = enabled;

        if (enabled)
        {
            if (Sound.SoundEnabled)
            {
                Sound.Tap.Play();
            }

            Sound.MainMusic.Play();
            PlayerPrefs.SetInt("Music", 1);
            PlayerPrefs.Save();
        }
        else
        {
            if (Sound.SoundEnabled)
            {
                Sound.Tap.Play();
            }

            Sound.MainMusic.Pause();
            PlayerPrefs.SetInt("Music", 0);
            PlayerPrefs.Save();
        }

        musicHandle.DOAnchorPos (enabled ? musicHandlePosition * -1 : musicHandlePosition, .4f).SetEase (Ease.InOutBack);
        musicBackgroundImage.DOColor (enabled ? backgroundActiveColor : backgroundDefaultColor, .6f);
        musicHandleImage.DOColor (enabled ? handleActiveColor : handleDefaultColor, .4f);
    }

    private void EnableSound(bool enabled)
    {
        Sound.SoundEnabled = enabled;

        if (enabled)
        {
            Sound.Tap.Play();
            PlayerPrefs.SetInt("Sound", 1);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 0);
            PlayerPrefs.Save();
        }

        soundHandle.DOAnchorPos(enabled ? soundHandlePosition * -1 : soundHandlePosition, .4f).SetEase(Ease.InOutBack);
        soundBackgroundImage.DOColor(enabled ? backgroundActiveColor : backgroundDefaultColor, .6f);
        soundHandleImage.DOColor(enabled ? handleActiveColor : handleDefaultColor, .4f);
    }

    private IEnumerator SetUI()
    {
        yield return new WaitForEndOfFrame();

        musicHandle = musicSwitch.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        soundHandle = soundSwitch.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        musicHandle.localPosition = new Vector2(-30 * musicSwitch.transform.GetChild(0).GetComponent<RectTransform>().rect.width / 160, musicHandle.localPosition.y);
        soundHandle.localPosition = new Vector2(musicHandle.localPosition.x, soundHandle.localPosition.y);

        musicHandlePosition = musicHandle.anchoredPosition;
        soundHandlePosition = soundHandle.anchoredPosition;

        musicBackgroundImage = musicHandle.parent.GetComponent<Image>();
        musicHandleImage = musicHandle.GetComponent<Image>();
        soundBackgroundImage = soundHandle.parent.GetComponent<Image>();
        soundHandleImage = soundHandle.GetComponent<Image>();

        backgroundDefaultColor = musicBackgroundImage.color;
        handleDefaultColor = musicHandleImage.color;

        if (PlayerPrefs.GetInt("Music", 1) == 1)
        {
            musicSwitch.isOn = true;

            musicHandle.DOAnchorPos(musicHandlePosition * -1, .4f).SetEase(Ease.InOutBack);
            musicBackgroundImage.DOColor(backgroundActiveColor, .6f);
            musicHandleImage.DOColor(handleActiveColor, .4f);
        }


        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            soundSwitch.isOn = true;

            soundHandle.DOAnchorPos(soundHandlePosition * -1, .4f).SetEase(Ease.InOutBack);
            soundBackgroundImage.DOColor(backgroundActiveColor, .6f);
            soundHandleImage.DOColor(handleActiveColor, .4f);
        }

        musicSwitch.onValueChanged.AddListener(EnableMusic);
        soundSwitch.onValueChanged.AddListener(EnableSound);
    }

    private void OnDestroy ()
    {
        musicSwitch.onValueChanged.RemoveListener (EnableMusic);
        soundSwitch.onValueChanged.RemoveListener(EnableMusic);
    }

    public void ChangeLocale(int localeID)
    {
        if (active)
        {
            return;
        }

        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int _localeID)
    {
        active = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("Locale", _localeID);

        Ticked(_localeID);

        active = false;
    }

    private void Ticked(int _localeID)
    {
        foreach (Transform button in Content)
        {
            button.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }

        Content.GetChild(_localeID).GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    private void OpenLanguagesPanel()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        LanguagesPanel.SetActive(true);
        LanguagesPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    private void CloseLanguagesPanel()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        LanguagesPanel.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => { LanguagesPanel.SetActive(false); });
    }

    private void RestorePurchases()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }
    }
}
