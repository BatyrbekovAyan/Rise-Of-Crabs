using UnityEngine;
using UnityEngine.Advertisements;
 
public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;

    private string adUnitId;

    private void Awake()
    {
        #if UNITY_IOS
                adUnitId = iosAdUnitId;
        #elif UNITY_ANDROID
                adUnitId = androidAdUnitId;
        #endif
    }

    public void LoadRewardedAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void ShowRewardedAd()
    {
        if (GameManager.adsInitialized)
        {
            if (Sound.MusicEnabled)
            {
                Sound.BackgroundMusic.Pause();
            }

            Advertisement.Show(adUnitId, this);
        }
    }

    public void OnUnityAdsAdLoaded(string adUnitId) { }
    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        LoadRewardedAd();
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        if (LevelWin.multiplied)
        {
            if (Levels.level % Creatures.sameLevelsNumber == 0)
            {
                PlayerPrefs.SetInt("Balls", PlayerPrefs.GetInt("Balls", 10) + 1);
                PlayerPrefs.SetInt("FreeBalls", PlayerPrefs.GetInt("FreeBalls", 0) - 1);
            }

            LevelWin.Instance.UpdateCoins();
        }
        else if (GameManager.gettingFreeHooks)
        {
            GameManager.Instance.GiveFreeHooks();
        }
        else if (GameManager.betweenLevelsAd)
        {
            GameManager.betweenLevelsAd = false;
            Time.timeScale = 1;
        }
        else if (Shop.getFreeBall)
        {
            Shop.getFreeBall = false;
            if (PlayerPrefs.GetInt("FreeBalls", 0) > 0)
            {
                PlayerPrefs.SetInt("Balls", PlayerPrefs.GetInt("Balls", 10) + 1);
                PlayerPrefs.SetInt("FreeBalls", PlayerPrefs.GetInt("FreeBalls", 0) - 1);
                Shop.FreeBallsTextStatic.text = PlayerPrefs.GetInt("FreeBalls", 0).ToString();
            }
        }
        else
        {
            GameOver.Instatnce.Continue();
        }

        if (Sound.MusicEnabled)
        {
            Sound.BackgroundMusic.UnPause();
        }

        LoadRewardedAd();
    }

    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            if (LevelWin.multiplied)
            {
                if (Levels.level % Creatures.sameLevelsNumber == 0)
                {
                    PlayerPrefs.SetInt("Balls", PlayerPrefs.GetInt("Balls", 10) + 1);
                    PlayerPrefs.SetInt("FreeBalls", PlayerPrefs.GetInt("FreeBalls", 0) - 1);
                }

                LevelWin.Instance.UpdateCoins();
            }
            else if (GameManager.gettingFreeHooks)
            {
                GameManager.Instance.GiveFreeHooks();
            }
            else if (GameManager.betweenLevelsAd)
            {
                GameManager.betweenLevelsAd = false;
                Time.timeScale = 1;
            }
            else if (Shop.getFreeBall)
            {
                Shop.getFreeBall = false;
                if (PlayerPrefs.GetInt("FreeBalls", 0) > 0)
                {
                    PlayerPrefs.SetInt("Balls", PlayerPrefs.GetInt("Balls", 10) + 1);
                    PlayerPrefs.SetInt("FreeBalls", PlayerPrefs.GetInt("FreeBalls", 0) - 1);
                    Shop.FreeBallsTextStatic.text = PlayerPrefs.GetInt("FreeBalls", 0).ToString();
                }
            }
            else
            {
                GameOver.Instatnce.Continue();
            }

            if (Sound.MusicEnabled)
            {
                Sound.BackgroundMusic.UnPause();
            }

            LoadRewardedAd();
        }

        else if(adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.SKIPPED))
        {
            if (LevelWin.multiplied)
            {
                LevelWin.Instance.DisableMultiply();
            }
            else if (GameManager.gettingFreeHooks)
            {
                GameManager.gettingFreeHooks = false;
            }
            else if (GameManager.betweenLevelsAd)
            {
                GameManager.betweenLevelsAd = false;
                Time.timeScale = 1;
            }
            else if (Shop.getFreeBall)
            {
                Shop.getFreeBall = false;
            }
            else
            {
                GameOver.Instatnce.DiisableContinue();
            }

            if (Sound.MusicEnabled)
            {
                Sound.BackgroundMusic.UnPause();
            }

            LoadRewardedAd();
        }
    }
}
