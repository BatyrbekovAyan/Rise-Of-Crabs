using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Balls;
    [SerializeField] private GameObject FreezeBackground;
    [SerializeField] private GameObject LevelStartPanel;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject LevelWinPanel;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject HooksPanel;
    [SerializeField] private GameObject GameOverTrigger;
    [SerializeField] private GameObject HouseRed;
    [SerializeField] private GameObject HouseBlue;
    [SerializeField] private GameObject SliderParent;
    [SerializeField] private GameObject CoinTop;
    [SerializeField] private GameObject ShooterBotsZone;
    [SerializeField] private GameObject NoInternet;
    [SerializeField] private GameObject TopBackground;
    [SerializeField] private GameObject TopBackgroundAlinedBtm;
    [SerializeField] private GameObject SceneBackground;
    [SerializeField] private GameObject ClawShooterSprite;
    [SerializeField] private TextMeshProUGUI CreaturesLeftText;
    [SerializeField] private TextMeshProUGUI RubysText;
    [SerializeField] private TextMeshProUGUI CoinsText;
    [SerializeField] private TextMeshProUGUI OperationMadeText;
    [SerializeField] private TextMeshProUGUI HooksNumberText;
    [SerializeField] private TextMeshProUGUI HouseRedLevelText;
    [SerializeField] private TextMeshProUGUI HouseBlueLevelText;
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button RubysButton;
    [SerializeField] private Button CoinsButton;
    [SerializeField] private Button HooksButton;
    [SerializeField] private Button BallsButton;
    [SerializeField] private Button FreezeTimeButton;
    [SerializeField] private Button DoubleCountingButton;
    [SerializeField] private Button PowerBallsButton;
    [SerializeField] private Button ResetBallsButton;
    [SerializeField] private Button SwitchHandButton;
    [SerializeField] private Button ShooterBotButton;
    [SerializeField] private Button BuyHooksButton;
    [SerializeField] private Button GetFreeHooksButton;
    [SerializeField] private Button CloseHooksPanelButton;
    [SerializeField] private Slider ProgressSlider;
    [SerializeField] private Slider Slider;
    [SerializeField] private Slider HooksSlider;
    [SerializeField] private Transform BottomPanel;
    [SerializeField] private Transform Scene;
    [SerializeField] private Sprite ClawSprite;
    [SerializeField] private Sprite ShooterSprite;
    [SerializeField] private Sprite HandleClaw;
    [SerializeField] private Sprite HandleShooter;
    [SerializeField] private List<Sprite> TopBackgrounds;
    [SerializeField] private List<Sprite> SceneBackgrounds;
    [SerializeField] private List<Color> TopBackgroundColors;
    [SerializeField] private List<Color> SceneBackgroundColors;
    [SerializeField] private List<Color> BottomBackgroundColors;

    public static GameObject CoinTopStatic;
    public static List<GameObject> psPlayingBotsList = new();
    public static int redCreaturesLeft;
    public static int adsForHookShown;
    public static bool levelWin;
    public static bool redHouseAttacked;
    public static bool doubleCounting;
    public static bool freezing;
    public static bool powerBalls;
    public static bool hookEnabled;
    public static bool gettingFreeHooks;
    public static bool betweenLevelsAd;
    public static bool adsInitialized = false;
    public static int allBotBluesTotalLifes;
    public static int allBotRedsTotalLifes;
    public static int freezingTime;
    public static int levelEranedCoins;
    public static Vector2 houseRedPosition;
    public static Vector2 houseBluePosition;
    public static TextMeshProUGUI OperationMadeTextStatic;
    public static TextMeshProUGUI RubysTextStatic;
    public static TextMeshProUGUI CoinsTextStatic;
    public static TextMeshProUGUI HouseBlueLevelTextStatic;

    private static float sliderNewValue;
    private static int totalCreaturesAndLifes;
    private int backgroundIndex;

    public static GameManager Instance;


    private void Start()
    {
        redHouseAttacked = false;
        doubleCounting = false;
        freezing = false;
        powerBalls = false;
        levelWin = false;
        hookEnabled = true;
        gettingFreeHooks = false;
        betweenLevelsAd = false;
        freezingTime = 5;
        levelEranedCoins = 0;
        CreaturesLeftText.text = redCreaturesLeft + "";
        houseRedPosition = HouseRed.transform.position;
        houseBluePosition = HouseBlue.transform.position;
        HouseRed.GetComponent<BoxCollider2D>().size = new Vector2(0.74f * HouseRed.GetComponent<RectTransform>().rect.size.y * HouseRed.GetComponent<Image>().sprite.rect.width / HouseRed.GetComponent<Image>().sprite.rect.height, HouseRed.GetComponent<RectTransform>().rect.size.y);
        HouseBlue.GetComponent<BoxCollider2D>().size = new Vector2(0.74f * HouseBlue.GetComponent<RectTransform>().rect.size.y * HouseBlue.GetComponent<Image>().sprite.rect.width / HouseBlue.GetComponent<Image>().sprite.rect.height, HouseBlue.GetComponent<RectTransform>().rect.size.y);
        psPlayingBotsList.Clear();

        SetUI();

        HouseBlueLevelTextStatic = HouseBlueLevelText;
        OperationMadeTextStatic = OperationMadeText;
        CoinTopStatic = CoinTop;

        UpdateSliderValue();
        ProgressSlider.value = sliderNewValue;
        Slider.transform.parent.GetComponent<Animation>().Play();

        if (!PlayerPrefs.HasKey("Rubys"))
        {
            PlayerPrefs.SetInt("Rubys", 500);
        }

        RubysText.text = PlayerPrefs.GetInt("Rubys", 500).ToString();
        CoinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString();
        RubysTextStatic = RubysText;
        CoinsTextStatic = CoinsText;

        adsForHookShown = PlayerPrefs.GetInt("AdsForHookShown", 0);

        if (adsForHookShown >= 4)
        {
            GetFreeHooksButton.transform.parent.gameObject.SetActive(false);
        }

        FreezeTimeButton.transform.parent.GetComponent<Animation>().Play();
        DoubleCountingButton.transform.parent.GetComponent<Animation>().Play();

        Instance = this;

        if (StartUI.adsOn && Creatures.levelRow > 32)
        {
            if (Creatures.levelRow > 60)
            {
                StartCoroutine(DisplayBannerWithDelay());
            }

            if (StartUI.gamePlayed % StartUI.adsInterval == 0)
            {
                betweenLevelsAd = true;
                Time.timeScale = 0;

                AdsManager.Instance.rewardedAds.ShowRewardedAd();

                StartUI.gamePlayed = 0;
            }

            StartUI.gamePlayed++;
        }


        if (PauseButton != null)
        {
            PauseButton.onClick.AddListener(PauseGame);
        }

        if (RubysButton != null)
        {
            RubysButton.onClick.AddListener(OpenShop);
        }

        if (CoinsButton != null)
        {
            CoinsButton.onClick.AddListener(OpenShop);
        }

        if (BallsButton != null)
        {
            BallsButton.onClick.AddListener(OpenShop);
        }

        if (HooksButton != null)
        {
            HooksButton.onClick.AddListener(OpenHooksPanel);
        }

        if (FreezeTimeButton != null)
        {
            FreezeTimeButton.transform.parent.localScale = 0.4f * Screen.height * FreezeTimeButton.transform.parent.localScale / Screen.width;
            FreezeTimeButton.onClick.AddListener(() => StartCoroutine(FreezeTime()));
        }

        if (DoubleCountingButton != null)
        {
            DoubleCountingButton.transform.parent.localScale = 0.4f * Screen.height * DoubleCountingButton.transform.parent.localScale / Screen.width;
            DoubleCountingButton.onClick.AddListener(() => StartCoroutine(DoubleCounting()));
        }

        if (PowerBallsButton != null)
        {
            PowerBallsButton.transform.parent.localScale = 0.4f * Screen.height * PowerBallsButton.transform.parent.localScale / Screen.width;
            PowerBallsButton.onClick.AddListener(() => StartCoroutine(PowerBalls()));
        }

        if (ResetBallsButton != null)
        {
            ResetBallsButton.transform.parent.localScale = 0.4f * Screen.height * ResetBallsButton.transform.parent.localScale / Screen.width;
        }

        if (SwitchHandButton != null)
        {
            SwitchHandButton.transform.parent.localScale = 0.4f * Screen.height * SwitchHandButton.transform.parent.localScale / Screen.width;
            SwitchHandButton.transform.parent.localPosition = 0.4f * Screen.height * SwitchHandButton.transform.parent.localPosition / Screen.width;
            SwitchHandButton.onClick.AddListener(SwitchHand);
        }

        if (ShooterBotButton != null)
        {
            ShooterBotButton.transform.parent.localScale = 0.4f * Screen.height * ShooterBotButton.transform.parent.localScale / Screen.width;
            ShooterBotButton.onClick.AddListener(ShooterBot);
        }

        if (BuyHooksButton != null)
        {
            BuyHooksButton.onClick.AddListener(BuyHooks);
        }

        if (GetFreeHooksButton != null)
        {
            GetFreeHooksButton.onClick.AddListener(GetFreeHooks);
        }

        if (CloseHooksPanelButton != null)
        {
            CloseHooksPanelButton.onClick.AddListener(CloseHooksPanel);
        }
    }

    private void FixedUpdate()
    {
        ProgressSlider.value = Mathf.MoveTowards(ProgressSlider.value, sliderNewValue, 200 * Time.deltaTime);
    }

    public void UpdateCrabsNumber(int number, string sign)
    {
        if (sign.Equals("-"))
        {
            redCreaturesLeft -= number;
        }
        else if(sign.Equals("+"))
        {
            redCreaturesLeft += number;
        }
        else if (sign.Equals("/"))
        {
            redCreaturesLeft /= number;
        }
        else if (sign.Equals("x"))
        {
            redCreaturesLeft *= number;
        }
        else if (sign.Equals("√x"))
        {
            if (redCreaturesLeft > 0)
            {
                redCreaturesLeft = (int)Mathf.Floor(Mathf.Sqrt(redCreaturesLeft));
            }
        }
        else if (sign.Equals("x²"))
        {
            redCreaturesLeft *= redCreaturesLeft;
            redCreaturesLeft = Mathf.Abs(redCreaturesLeft);
        }


        for (int i = 5; i > 1; i--)
        {
            if (redCreaturesLeft >= (int)Mathf.Pow(10, i))
            {
                if (Creatures.botRedLevel < i)
                {
                    Creatures.botRedLevel = i;
                    break;
                }
            }
        }

        if (redCreaturesLeft > 100000)
        {
            redCreaturesLeft = 100000;
        }
        else if (redCreaturesLeft < 0)
        {
            redCreaturesLeft = 0;
        }

        UpdateCreaturesLeftText();

        if (Creatures.levelRow > 16 && !Creatures.movePortals)
        {
            Creatures.movePortals = true;
        }
    }

    public static void UpdateSliderValue()
    {
        totalCreaturesAndLifes = 0;

        allBotBluesTotalLifes = 0;
        allBotRedsTotalLifes = 0;

        foreach (GameObject botBlue in Creatures.botBlueList)
        {
            allBotBluesTotalLifes += botBlue.GetComponent<BotBlue>().currentLife;
        }

        foreach (GameObject botRed in Creatures.botRedList)
        {
            allBotRedsTotalLifes += botRed.GetComponent<BotRed>().currentLife;
        }

        totalCreaturesAndLifes = allBotBluesTotalLifes + allBotRedsTotalLifes;

        if (Creatures.blueCreaturesTotal > 0)
        {
            totalCreaturesAndLifes += Creatures.blueCreaturesTotal;
        }

        if (redCreaturesLeft > 0)
        {
            totalCreaturesAndLifes += redCreaturesLeft;
        }

        if (totalCreaturesAndLifes != 0)
        {
            sliderNewValue = (Creatures.blueCreaturesTotal + allBotBluesTotalLifes) * 1000 / totalCreaturesAndLifes;
        }
    }

    public void GameOver()
    {
        GameOverTrigger.GetComponent<GameOver>().ShowGameOverPanel();
    }

    public void LevelWin()
    {
        if (!levelWin)
        {
            levelWin = true;
            LevelWinPanel.GetComponent<LevelWin>().ShowLevelWinPanel();
        }
    }

    public void UpdateCreaturesLeftText()
    {
        for (int i = 5; i >= 0; i--)
        {
            if (redCreaturesLeft >= (int)Mathf.Pow(10, i))
            {
                if (i == 0)
                {
                    i = 1;
                }
                if (!HouseRedLevelText.text.Equals((2 * i + Creatures.levelRow / 16).ToString()))
                {
                    HouseRedLevelText.text = (2 * i + Creatures.levelRow / 16).ToString();
                    HouseRedLevelText.GetComponent<Animation>().Play();
                }

                break;
            }
        }

        CreaturesLeftText.text = redCreaturesLeft + "";
    }

    private void PauseGame()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        PausePanel.SetActive(true);
    }

    public void OpenShop()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        PauseGame();

        StartUI.ShopOpenedIngame = true;
        StartUI.ShowStartUI();
        StartUI.ShopPanelStatic.SetActive(true);
    }

    private IEnumerator DoubleCounting()
    {
        if (PlayerPrefs.GetInt("Rubys", 0) >= 5)
        {
            if (Sound.SoundEnabled)
            {
                Sound.Tap.Play();
            }

            doubleCounting = true;
            DoubleCountingButton.interactable = false;

            foreach (GameObject creature in Creatures.creaturesBodiesList)
            {
                if (creature != null)
                {
                    if (creature.GetComponent<ICreatureBody>().GetNumber() != -1 && creature.CompareTag("Target"))
                    {
                        creature.GetComponent<ICreatureBody>().SetNumber(2);
                    }
                }
            }

            PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) - 5);
            PlayerPrefs.Save(); 
            RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();

            for (int i = 10; i > 0; i--)
            {
                DoubleCountingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.ToString();
                DoubleCountingButton.GetComponent<Animation>().Play();

                yield return new WaitForSeconds(1f);
            }

            DoubleCountingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

            doubleCounting = false;

            foreach (GameObject creature in Creatures.creaturesBodiesList)
            {
                if (creature != null)
                {
                    if (creature.GetComponent<ICreatureBody>().GetNumber() != -1 && creature.CompareTag("Target"))
                    {
                        creature.GetComponent<ICreatureBody>().SetNumber(0.5f);
                    }
                }
            }

            DoubleCountingButton.GetComponent<Image>().color = Color.white;

            DoubleCountingButton.interactable = true;
        }
        else
        {
            OpenShop();
        }
    }

    private IEnumerator FreezeTime()
    {
        if (PlayerPrefs.GetInt("Rubys", 0) >= 5)
        {
            if (Sound.SoundEnabled)
            {
                Sound.Tap.Play();
            }

            freezing = true;
            FreezeBackground.SetActive(true);
            FreezeTimeButton.interactable = false;
            FreezeTimeButton.GetComponent<Animation>().Play();

            PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) - 5);
            PlayerPrefs.Save();
            RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();

            for (int i = freezingTime; i > 0; i--)
            {
                FreezeTimeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            FreezeTimeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

            freezing = false;
            FreezeBackground.SetActive(false);
            FreezeTimeButton.interactable = true;
        }
        else
        {
            OpenShop();
        }
    }

    private IEnumerator PowerBalls()
    {
        if (PlayerPrefs.GetInt("Rubys", 0) >= 2)
        {
            if (Sound.SoundEnabled)
            {
                Sound.Tap.Play();
            }

            powerBalls = true;
            PowerBallsButton.interactable = false;
            Shooter.FirstBallSpriteStatic.GetComponent<Image>().color = Color.blue;
            PowerBallsButton.GetComponent<Animation>().Play("PowerBallTurnBlue");

            foreach (Transform ball in Balls.transform)
            {
                ball.GetComponent<Ball>().ballPower = 4;
                ball.GetComponent<Image>().color = Color.blue;
            }

            PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) - 2);
            PlayerPrefs.Save();
            RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();

            yield return new WaitForSeconds(0.6f);

            for (int i = 10; i > 0; i--)
            {
                PowerBallsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            PowerBallsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

            powerBalls = false;
            PowerBallsButton.interactable = true;
            Shooter.FirstBallSpriteStatic.GetComponent<Image>().color = Color.red;
            PowerBallsButton.GetComponent<Animation>().Play("PowerBallTurnRed");

            foreach (Transform ball in Balls.transform)
            {
                ball.GetComponent<Ball>().ballPower = 1;
                ball.GetComponent<Image>().color = Color.red;
            }
        }
        else
        {
            OpenShop();
        }
    }

    private void SwitchHand()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        if (!hookEnabled || Hooker.switchedToShooter)
        {
            hookEnabled = true;
            Hooker.switchedToShooter = false;

            ClawShooterSprite.GetComponent<Image>().sprite = ClawSprite; 
            Slider.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = HandleClaw;


            FreezeTimeButton.transform.parent.GetComponent<Canvas>().sortingOrder = 3;
            if (!freezing)
            {
                FreezeTimeButton.interactable = true;
            }
            DoubleCountingButton.transform.parent.GetComponent<Canvas>().sortingOrder = 3;
            if (!doubleCounting)
            {
                DoubleCountingButton.interactable = true;
            }
            PowerBallsButton.transform.parent.GetComponent<Canvas>().sortingOrder = 0;
            PowerBallsButton.interactable = false;
            ResetBallsButton.transform.parent.gameObject.SetActive(false);

            foreach (GameObject creature in Creatures.creaturesBodiesList)
            {
                if (creature != null && creature.transform.childCount > 1)
                {
                    creature.transform.GetChild(0).gameObject.SetActive(true);
                    creature.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (Hooker.isHooking)
            {
                Hooker.switchedToShooter = true;
            }
            else
            {
                hookEnabled = false;
            }

            ClawShooterSprite.GetComponent<Image>().sprite = ShooterSprite;
            Slider.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = HandleShooter;

            FreezeTimeButton.transform.parent.GetComponent<Canvas>().sortingOrder = 0;
            FreezeTimeButton.interactable = false;
            DoubleCountingButton.transform.parent.GetComponent<Canvas>().sortingOrder = 0;
            DoubleCountingButton.interactable = false;
            PowerBallsButton.transform.parent.GetComponent<Canvas>().sortingOrder = 3;
            if (!powerBalls)
            {
                PowerBallsButton.interactable = true;
            }
            ResetBallsButton.transform.parent.gameObject.SetActive(true);

            foreach (GameObject creature in Creatures.creaturesBodiesList)
            {
                if (creature != null && creature.transform.childCount > 1)
                {
                    creature.transform.GetChild(0).gameObject.SetActive(false);
                    creature.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
    }

    private void ShooterBot()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        ShooterBotsZone.SetActive(!ShooterBotsZone.activeSelf);
    }

    private void BuyHooks()
    {
        if (PlayerPrefs.GetInt("Rubys", 0) >= 100)
        {
            if (Sound.SoundEnabled)
            {
                Sound.Tap.Play();
            }

            Hooker.hooksNumber += 100;
            PlayerPrefs.SetInt("Hooks", Hooker.hooksNumber);
            HooksNumberText.text = Hooker.hooksNumber.ToString();
            HooksSlider.value = Hooker.hooksNumber;

            Hooker.Instance.UpdateTimerText();

            PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) - 100);
            RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();
            HooksPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            HooksPanel.SetActive(false);
            OpenShop();
        }
    }

    private void GetFreeHooks()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        if (StartUI.internetConnected)
        {
            adsForHookShown++;
            PlayerPrefs.SetInt("AdsForHookShown", adsForHookShown);

            if (adsForHookShown >= 4)
            {
                GetFreeHooksButton.transform.parent.gameObject.SetActive(false);
            }

            gettingFreeHooks = true;
            AdsManager.Instance.rewardedAds.ShowRewardedAd();
        }
        else
        {
            NoInternet.SetActive(true);

            DiactivateNoInternetPanel();
        }
    }

    public void GiveFreeHooks()
    {
        Hooker.hooksNumber += 25;
        PlayerPrefs.SetInt("Hooks", Hooker.hooksNumber);
        HooksNumberText.text = Hooker.hooksNumber.ToString();
        HooksSlider.value = Hooker.hooksNumber;

        Hooker.Instance.UpdateTimerText();

        HooksPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenHooksPanel()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        foreach (GameObject creature in Creatures.creaturesBodiesList)
        {
            if (creature.transform.childCount > 2)
            {
                creature.transform.GetChild(2).GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
            }
            else if (creature.transform.childCount > 1)
            {
                creature.transform.GetChild(1).GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
            }
        }

        HooksPanel.SetActive(true);
        Time.timeScale = 0;
    }

    private void CloseHooksPanel()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        foreach (GameObject creature in Creatures.creaturesBodiesList)
        {
            if (creature != null)
            {
                if (creature.transform.childCount > 2)
                {
                    creature.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1);
                }
                else if (creature.transform.childCount > 1)
                {
                    creature.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1);
                }
            }
        }

        HooksPanel.SetActive(false);
        Time.timeScale = 1;
    }

    private static IEnumerator DisplayBannerWithDelay()
    {
        yield return new WaitForSeconds(5f);
        AdsManager.Instance.bannerAds.ShowBannerAd();
    }

    private void SetUI()
    {
        backgroundIndex = Mathf.FloorToInt((Levels.level - 1) / 12);
        if (backgroundIndex >= TopBackgrounds.Count)
        {
            backgroundIndex %= TopBackgrounds.Count;
        }

        TopBackground.GetComponent<Image>().sprite = TopBackgrounds[backgroundIndex];
        TopBackgroundAlinedBtm.GetComponent<Image>().sprite = TopBackgrounds[backgroundIndex];
        SceneBackground.GetComponent<Image>().sprite = SceneBackgrounds[backgroundIndex];
        TopBackground.GetComponent<Image>().color = TopBackgroundColors[backgroundIndex];
        TopBackgroundAlinedBtm.GetComponent<Image>().color = TopBackgroundColors[backgroundIndex];
        SceneBackground.GetComponent<Image>().color = SceneBackgroundColors[backgroundIndex];
        BottomPanel.GetComponent<Image>().color = BottomBackgroundColors[backgroundIndex];

        if (9 <= Creatures.levelRow % 64 && Creatures.levelRow % 64 <= 12 ||
            21 <= Creatures.levelRow % 64 && Creatures.levelRow % 64 <= 24 ||
            25 <= Creatures.levelRow % 64 && Creatures.levelRow % 64 <= 28 ||
            49 <= Creatures.levelRow % 64 && Creatures.levelRow % 64 <= 52)
        {
            TopBackgroundAlinedBtm.SetActive(true);
            TopBackground.SetActive(false);
        }

        float sceneRatio = Scene.GetComponent<RectTransform>().rect.width / Scene.GetComponent<RectTransform>().rect.height;
        float spriteRatio = SceneBackground.GetComponent<Image>().sprite.rect.width / SceneBackground.GetComponent<Image>().sprite.rect.height;

        if (sceneRatio < spriteRatio)
        {
            SceneBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(Scene.GetComponent<RectTransform>().rect.height * spriteRatio, Scene.GetComponent<RectTransform>().rect.height);
        }
        else
        {
            SceneBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(Scene.GetComponent<RectTransform>().rect.width, Scene.GetComponent<RectTransform>().rect.width / spriteRatio);
        }
    }

    private void OnApplicationPause(bool appInBackground)
    {
        if (appInBackground)
        {
            if (!GameOverPanel.activeSelf && !LevelWinPanel.activeSelf && !LevelStartPanel.activeSelf && !HooksPanel.activeSelf && !PausePanel.activeSelf)
            {
                PauseGame();
            }
        }
    }

    private async void DiactivateNoInternetPanel()
    {
        await Task.Delay(2000);

        NoInternet.SetActive(false);
    }
}
