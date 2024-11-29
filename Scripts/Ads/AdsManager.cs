using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public InitializeAds InitializeAds;
    public InterstitialAds interstitialAds;
    public RewardedAds rewardedAds;
    public BannerAds bannerAds;

    public static AdsManager Instance { get; private set; }


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
}
