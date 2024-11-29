using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using AdjustSdk;

public class LevelWin : MonoBehaviour
{
    [SerializeField] private GameObject LevelWinText;
    [SerializeField] private GameObject VictoryText;
    [SerializeField] private GameObject CoinMain;
    [SerializeField] private GameObject CoinAnimated;
    [SerializeField] private GameObject Clapperboard;
    [SerializeField] private GameObject NoInternet;
    [SerializeField] private GameObject BallMain;
    [SerializeField] private GameObject AddBall;
    [SerializeField] private GameObject AddBallImageAnimated;
    [SerializeField] private Transform ShooterBots;
    [SerializeField] private Button LevelWinMultiplyButton;
    [SerializeField] private Button LevelWinNextLevelButton;
    [SerializeField] private TextMeshProUGUI LevelWinCoinsText;

    public static bool multiplied;
    private int levelWinCoins;
    private float animatedCoinsScale;
    private float animatedBallsScale;

    private List<Task> tasks = new();

    public static LevelWin Instance;


    void Start()
    {
        LevelWinCoinsText.transform.localPosition = new Vector2(19.5f + 4.9f * Screen.height / Screen.width, LevelWinCoinsText.transform.localPosition.y);

        animatedCoinsScale = CoinMain.transform.GetComponent<RectTransform>().rect.height / CoinAnimated.transform.GetComponent<RectTransform>().rect.height;
        animatedBallsScale = BallMain.transform.GetComponent<RectTransform>().rect.height / AddBallImageAnimated.transform.GetComponent<RectTransform>().rect.height;

        Instance = this;

        multiplied = false;

        if (LevelWinMultiplyButton != null)
        {
            LevelWinMultiplyButton.onClick.AddListener(LevelWinMultiply);
        }

        if (LevelWinNextLevelButton != null)
        {
            LevelWinNextLevelButton.onClick.AddListener(LevelWinNextLevel);
        }
    }


    public void ShowLevelWinPanel()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Win.Play();
        }

        levelWinCoins = 0;

        foreach (GameObject botRed in Creatures.botRedList)
        {
            levelWinCoins += botRed.GetComponent<BotRed>().currentLife;
        }

        levelWinCoins = levelWinCoins + (int)Mathf.Sqrt(GameManager.redCreaturesLeft) + GameManager.levelEranedCoins;

        if (levelWinCoins < 0)
        {
            levelWinCoins = 0;
        }

        if (Levels.level % Creatures.sameLevelsNumber == 0)
        {
            LevelWinText.SetActive(true);
            VictoryText.SetActive(false);

            AddBall.SetActive(true);

            PlayerPrefs.SetInt("FreeBalls", PlayerPrefs.GetInt("FreeBalls", 0) + 1);
        }

        gameObject.SetActive(true);

        for (int i = 1; i < ShooterBots.childCount; i++)
        {
            StartCoroutine(WeeeingCrab(ShooterBots.GetChild(i), false));
        }

        if (StartUI.adsOn && Creatures.levelRow > 60)
        {
            AdsManager.Instance.bannerAds.HideBannerAd();
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

        UpdateCoins();

        UnlockNewLevel();
        Time.timeScale = 0;
    }

    private void LevelWinMultiply()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        if (StartUI.internetConnected)
        {
            multiplied = true;

            AdsManager.Instance.rewardedAds.ShowRewardedAd();
        }
        else
        {
            NoInternet.SetActive(true);

            DiactivateNoInternetPanel();
        }
    }

    private void LevelWinNextLevel()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        Levels.level++;
        LevelTranslation.level = Mathf.Floor((Levels.level - 1) / 3 + 1).ToString();
        GameManager.levelWin = false;
        Time.timeScale = 1;

        SceneManager.LoadScene("Main");
    }

    private void UnlockNewLevel()
    {
        if (Levels.level >= PlayerPrefs.GetInt("UnlockedLevel"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();

            AdjustEvent levelAchievedEvent = new AdjustEvent("d54cp8");
            Adjust.TrackEvent(levelAchievedEvent);
        }
    }

    public async void UpdateCoins()
    {
        LevelWinNextLevelButton.interactable = false;

        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + levelWinCoins);
        PlayerPrefs.Save();

        if (multiplied)
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + levelWinCoins);
            PlayerPrefs.Save();

            multiplied = false;
            LevelWinMultiplyButton.interactable = false;

            levelWinCoins *= 3;


            if (Levels.level % Creatures.sameLevelsNumber == 0)
            {
                AddBallImageAnimated.SetActive(true);
                AddBallImageAnimated.transform.DOMove(BallMain.transform.position, 1f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
                {
                    AddBallImageAnimated.transform.DOScale(animatedBallsScale, 0.5f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
                    {
                        AddBallImageAnimated.GetComponent<Image>().DOFade(0, 1f).SetUpdate(true);
                    });
                });
            }
        }
        else
        {
            CoinAnimated.transform.localPosition = new Vector2(CoinAnimated.transform.localPosition.x + 198.775f - 70.446f * Screen.height / Screen.width, CoinAnimated.transform.localPosition.y);
        }

        LevelWinCoinsText.text = levelWinCoins.ToString();


        GameObject coinInstance1 = Instantiate(CoinAnimated, CoinAnimated.transform.position, CoinAnimated.transform.rotation, CoinAnimated.transform.parent.transform);
        GameObject coinInstance2 = Instantiate(CoinAnimated, CoinAnimated.transform.position, CoinAnimated.transform.rotation, CoinAnimated.transform.parent.transform);
        GameObject coinInstance3 = Instantiate(CoinAnimated, CoinAnimated.transform.position, CoinAnimated.transform.rotation, CoinAnimated.transform.parent.transform);
        GameObject coinInstance4 = Instantiate(CoinAnimated, CoinAnimated.transform.position, CoinAnimated.transform.rotation, CoinAnimated.transform.parent.transform);
        GameObject coinInstance5 = Instantiate(CoinAnimated, CoinAnimated.transform.position, CoinAnimated.transform.rotation, CoinAnimated.transform.parent.transform);

        coinInstance1.SetActive(true);
        tasks.Add(coinInstance1.transform.DOMove(CoinMain.transform.position, 1f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            coinInstance1.transform.DOScale(animatedCoinsScale, 0.5f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => { Sound.CoinsDrop.Play(); }); }).AsyncWaitForCompletion());
        await Task.Delay(100);
        coinInstance2.SetActive(true);
        tasks.Add(coinInstance2.transform.DOMove(CoinMain.transform.position, 1f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            coinInstance2.transform.DOScale(animatedCoinsScale, 0.5f).SetEase(Ease.InBack).SetUpdate(true);}).AsyncWaitForCompletion());
        await Task.Delay(100);
        coinInstance3.SetActive(true);
        tasks.Add(coinInstance3.transform.DOMove(CoinMain.transform.position, 1f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            coinInstance3.transform.DOScale(animatedCoinsScale, 0.5f).SetEase(Ease.InBack).SetUpdate(true);}).AsyncWaitForCompletion());
        await Task.Delay(100);
        coinInstance4.SetActive(true);
        tasks.Add(coinInstance4.transform.DOMove(CoinMain.transform.position, 1f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            coinInstance4.transform.DOScale(animatedCoinsScale, 0.5f).SetEase(Ease.InBack).SetUpdate(true);}).AsyncWaitForCompletion());
        await Task.Delay(100);
        coinInstance5.SetActive(true);
        tasks.Add(coinInstance5.transform.DOMove(CoinMain.transform.position, 1f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            coinInstance5.transform.DOScale(animatedCoinsScale, 0.5f).SetEase(Ease.InBack).SetUpdate(true);}).AsyncWaitForCompletion());
        await Task.WhenAll(tasks);
        await Task.Delay(500);

        if (coinInstance1 && coinInstance2 && coinInstance3 && coinInstance4 && coinInstance5)
        {
            coinInstance1.GetComponent<Image>().DOFade(0, 0.5f).SetUpdate(true);
            coinInstance2.GetComponent<Image>().DOFade(0, 0.5f).SetUpdate(true);
            coinInstance3.GetComponent<Image>().DOFade(0, 0.5f).SetUpdate(true);
            coinInstance4.GetComponent<Image>().DOFade(0, 0.5f).SetUpdate(true);
            coinInstance5.GetComponent<Image>().DOFade(0, 0.5f).SetUpdate(true)
                .OnComplete(() =>
                {
                    LevelWinNextLevelButton.interactable = true;
                    GameManager.CoinsTextStatic.text = PlayerPrefs.GetInt("Coins", 0).ToString();
                });
        }
    }

    public void DisableMultiply()
    {
        multiplied = false;
        LevelWinMultiplyButton.interactable = false;
    }

    private async void DiactivateNoInternetPanel()
    {
        await Task.Delay(2000);

        NoInternet.SetActive(false);
    }

    private IEnumerator WeeeingCrab(Transform ShooterBot, bool crabAtFront)
    {
        yield return new WaitForSecondsRealtime(1f + 2 * Random.value);

        if (!crabAtFront)
        {
            ShooterBot.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder += 20;

            yield return new WaitForSecondsRealtime(0.1f);

            foreach (Transform bodyPart in ShooterBot.GetChild(1).GetChild(0).GetChild(0))
            {
                yield return new WaitForSecondsRealtime(0.1f);

                bodyPart.GetComponent<SpriteRenderer>().sortingOrder += 20;
            }

            ShooterBot.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(true);
            ShooterBot.GetComponent<Animator>().enabled = true;
        }

        if (Sound.SoundEnabled)
        {
            Sound.Weee.PlayOneShot(Sound.Weee.clip);
        }

        StartCoroutine(WeeeingCrab(ShooterBot, true));
    }
}
