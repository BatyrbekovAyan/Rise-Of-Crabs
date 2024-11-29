using UnityEngine;
using UnityEngine.Advertisements;
 
public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
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

    public void LoadInterstitialAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void ShowInterstitialAd()
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
        LoadInterstitialAd();
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        if (Sound.MusicEnabled)
        {
            Sound.BackgroundMusic.UnPause();
        }

        LoadInterstitialAd();
    }

    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (Sound.MusicEnabled)
        {
            Sound.BackgroundMusic.UnPause();
        }

        LoadInterstitialAd();
    }
}
