using UnityEngine;
using UnityEngine.Advertisements;
 
public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameId;
    [SerializeField] private string iosGameId;
    [SerializeField] private bool isTesting;

    private string gameId;

    void Awake()
    {
        #if UNITY_IOS
                gameId = iosGameId;
        #elif UNITY_ANDROID
                gameId = androidGameId;
        #elif UNITY_EDITOR
                gameId = iosGameId; //Only for testing the functionality in the Editor
        #endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, isTesting, this);
        }
    }

    public void OnInitializationComplete()
    {
        GameManager.adsInitialized = true;
        AdsManager.Instance.interstitialAds.LoadInterstitialAd();
        AdsManager.Instance.rewardedAds.LoadRewardedAd();
        AdsManager.Instance.bannerAds.LoadBannerAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, isTesting, this);
        }
    }
}