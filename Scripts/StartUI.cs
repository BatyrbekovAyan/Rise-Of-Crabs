using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button LevelsButton;
    [SerializeField] private Button CloseLevelsButton;
    [SerializeField] private Button ShopButton;
    [SerializeField] private Button CloseShopButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button CloseSettingsButton;
    [SerializeField] private GameObject LevelsPanel;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject RemoveAdsPanel;
    [SerializeField] private GameObject RestorePurchases;
    [SerializeField] private GameObject LocalizationContent;
    [SerializeField] private Image Background;

    private static bool created = false;

    public static int canvasHeight;

    public static bool adsOn;
    public static bool internetConnected = false;
    public static bool ShopOpenedIngame = false;
    public static bool levelsPanelOpen = false;
    public static GameObject StartUIInstance;
    public static GameObject ShopPanelStatic;
    private static GameObject LevelsPanelStatic;

    public static int gamePlayed = 1;
    public static int adsInterval = 10;


    private void Start()
    {
        if (created)
        {
            Destroy(gameObject);
        }

        else
        {
            DontDestroyOnLoad(gameObject);
            created = true;
            Application.targetFrameRate = 60;

            SetUI();

            StartUIInstance = gameObject;
            ShopPanelStatic = ShopPanel;
            LevelsPanelStatic = LevelsPanel;

            //PlayerPrefs.SetString("LastFreeHooksDate", "08/10/2024 00:00:00");
            //PlayerPrefs.SetInt("AdsForHookShown", 0);
            //PlayerPrefs.SetInt("AdsOn", 1);
            if (PlayerPrefs.GetInt("AdsOn", 1) == 1)
            {
                adsOn = true;
            }

            else
            {
                adsOn = false;
                RemoveAdsPanel.SetActive(false);
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                RestorePurchases.SetActive(true);
            }

            StartCoroutine(SetLocale());

            StartCoroutine(CheckInternetConnection());


            if (PlayButton != null)
            {
                PlayButton.onClick.AddListener(PlayGame);
            }

            if (LevelsButton != null)
            {
                LevelsButton.onClick.AddListener(ShowLevels);
            }

            if (CloseLevelsButton != null)
            {
                CloseLevelsButton.onClick.AddListener(CloseLevels);
            }

            if (ShopButton != null)
            {
                ShopButton.onClick.AddListener(ShowShop);
            }

            if (CloseShopButton != null)
            {
                CloseShopButton.onClick.AddListener(CloseShop);
            }

            if (SettingsButton != null)
            {
                SettingsButton.onClick.AddListener(ShowSettings);
            }

            if (CloseSettingsButton != null)
            {
                CloseSettingsButton.onClick.AddListener(CloseSettings);
            }
        }
    }


    private void PlayGame()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        Levels.level = PlayerPrefs.GetInt("UnlockedLevel", 1);
        LevelTranslation.level = Mathf.Floor((Levels.level - 1) / 3 + 1).ToString();

        SceneManager.LoadScene("Main");
    }

    private void ShowLevels()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        LevelsPanel.SetActive(true);
    }

    private void CloseLevels()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        LevelsPanel.SetActive(false);
    }

    private void ShowShop()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        ShopPanel.SetActive(true);
    }

    private void CloseShop()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        ShopPanel.SetActive(false);

        if (ShopOpenedIngame)
        {
            ShopOpenedIngame = false;

            GameManager.RubysTextStatic.text = PlayerPrefs.GetInt("Rubys", 0).ToString();
            GameManager.CoinsTextStatic.text = PlayerPrefs.GetInt("Coins", 0).ToString();

            StartUIInstance.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void ShowSettings()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        SettingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        SettingsPanel.SetActive(false);
    }

    public static void ShowStartUI()
    {
        if (levelsPanelOpen)
        {
            levelsPanelOpen = false;
            LevelsPanelStatic.SetActive(false);
        }

        if (Sound.MusicEnabled && !ShopOpenedIngame)
        {
            Sound.BackgroundMusic.Stop();
            Sound.MainMusic.Play();
        }

        if (adsOn && Creatures.levelRow > 60)
        {
            AdsManager.Instance.bannerAds.HideBannerAd();
        }

        StartUIInstance.transform.GetChild(0).gameObject.SetActive(true);
    }

    public static void HideStartUI()
    {
        if (Sound.MusicEnabled)
        {
            if (Sound.BackgroundMusic.isPlaying)
            {
                return;
            }

            Sound.MainMusic.Stop();
            Sound.BackgroundMusic.PlayOneShot(Sound.Instance.MusicClipsList[Random.Range(0, Sound.Instance.MusicClipsList.Count)]);
        }

        StartUIInstance.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void SetUI()
    {
        canvasHeight = 1200 * Screen.height / Screen.width;
        float aspectRatio = Background.sprite.rect.height / Background.sprite.rect.width;
        Background.rectTransform.sizeDelta = new Vector2(canvasHeight / aspectRatio, canvasHeight);

        LevelsPanel.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(canvasHeight / aspectRatio, canvasHeight);
        ShopPanel.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(canvasHeight / aspectRatio, canvasHeight);
        SettingsPanel.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(canvasHeight / aspectRatio, canvasHeight);

        if (LevelsPanel.transform.GetChild(0).GetComponent<RectTransform>().rect.width > LevelsPanel.GetComponent<RectTransform>().rect.width)
        {
            LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(LevelsPanel.GetComponent<RectTransform>().rect.width, LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
            ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(ShopPanel.GetComponent<RectTransform>().rect.width, ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
            SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(SettingsPanel.GetComponent<RectTransform>().rect.width, SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
        }
        else
        {
            LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(LevelsPanel.transform.GetChild(0).GetComponent<RectTransform>().rect.width / 2, LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
            ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(ShopPanel.transform.GetChild(0).GetComponent<RectTransform>().rect.width / 2, ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
            SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(SettingsPanel.transform.GetChild(0).GetComponent<RectTransform>().rect.width / 2, SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
        }

        LevelsPanel.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().cellSize = new Vector2(LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 6, LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 6);
        LevelsPanel.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().spacing = new Vector2(LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 30, LevelsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 30);

        ShopPanel.transform.GetChild(1).GetChild(0).GetComponent<GridLayoutGroup>().cellSize = new Vector2(2 * ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 3, ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 3);
        ShopPanel.transform.GetChild(1).GetChild(0).GetComponent<GridLayoutGroup>().spacing = new Vector2(0, ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 10);
        ShopPanel.transform.GetChild(1).GetChild(0).GetComponent<GridLayoutGroup>().padding.top = (int) ShopPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 10;

        SettingsPanel.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(2 * SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 3, SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 3);
        SettingsPanel.transform.GetChild(1).GetChild(0).localPosition = new Vector2(0, -SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().rect.height / 10);

        SettingsPanel.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(2 * SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 3, SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 6);
        SettingsPanel.transform.GetChild(1).GetChild(1).localPosition = new Vector2(0, -SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().rect.height / 10 - SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 2.4f);

        SettingsPanel.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 2.5f, SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 10);
        SettingsPanel.transform.GetChild(1).GetChild(2).localPosition = new Vector2(0, -SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().rect.height / 10 - SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 1.5f);

        SettingsPanel.transform.GetChild(1).GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(2 * SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 3, 0.9f *  SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x);
        SettingsPanel.transform.GetChild(1).GetChild(3).localPosition = new Vector2(0, -SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().rect.height / 10);

        LocalizationContent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 2, SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 7.5f);
        LocalizationContent.GetComponent<GridLayoutGroup>().spacing = new Vector2(0, SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 60);
        LocalizationContent.GetComponent<GridLayoutGroup>().padding.top = (int)SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 75;
        LocalizationContent.GetComponent<GridLayoutGroup>().padding.bottom = (int)SettingsPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x / 40;
    }

    private IEnumerator SetLocale()
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("Locale", 0)];

        LocalizationContent.transform.GetChild(PlayerPrefs.GetInt("Locale", 0)).GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    private IEnumerator CheckInternetConnection()
    {
        UnityWebRequest request = new("https://google.com");

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            internetConnected = false;
        }
        else
        {
            internetConnected = true;
        }

        yield return new WaitForSecondsRealtime(30f);

        StartCoroutine(CheckInternetConnection());
    }
}
