using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using AdjustSdk;

public class Shop : MonoBehaviour, IDetailedStoreListener
{
    [SerializeField] private Button RemoveAdsButton;
    [SerializeField] private Button ChangeButton;
    [SerializeField] private Button GetBallButton;
    [SerializeField] private Button BuyCrystalPack1Button;
    [SerializeField] private Button BuyCrystalPack2Button;
    [SerializeField] private TextMeshProUGUI RubysText;
    [SerializeField] private TextMeshProUGUI CoinsText;
    [SerializeField] private TextMeshProUGUI FreeBallsText;
    [SerializeField] private NonConsumableItem RemoveAdsItem;
    [SerializeField] private ConsumableItem CrystalsPack1;
    [SerializeField] private ConsumableItem CrystalsPack2;

    public static bool getFreeBall;
    public static TextMeshProUGUI FreeBallsTextStatic;

    IStoreController m_StoreContoller;


    private void Start()
    {
        if (!PlayerPrefs.HasKey("Rubys"))
        {
            PlayerPrefs.SetInt("Rubys", 500);
        }

        getFreeBall = false;

        RubysText.text = PlayerPrefs.GetInt("Rubys", 500).ToString();
        CoinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString();

        FreeBallsTextStatic = FreeBallsText;

        SetupBuilder();


        if (RemoveAdsButton != null)
        {
            RemoveAdsButton.onClick.AddListener(RemoveAds);
        }

        if (ChangeButton != null)
        {
            ChangeButton.onClick.AddListener(Change);
        }

        if (GetBallButton != null)
        {
            GetBallButton.onClick.AddListener(GetBall);
        }

        if (BuyCrystalPack1Button != null)
        {
            BuyCrystalPack1Button.onClick.AddListener(BuyCrystalPack1);
        }

        if (BuyCrystalPack2Button != null)
        {
            BuyCrystalPack2Button.onClick.AddListener(BuyCrystalPack2);
        }
    }

    private void OnEnable()
    {
        if (Sound.SoundEnabled)
        {
            Sound.OpenShop.Play();
        }

        RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();
        CoinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString();

        FreeBallsText.text = PlayerPrefs.GetInt("FreeBalls", 0).ToString();

        if (PlayerPrefs.GetInt("Coins", 0) >= 1000)
        {
            ChangeButton.interactable = true;
        }
        else
        {
            ChangeButton.interactable = false;
        }

        if (PlayerPrefs.GetInt("FreeBalls", 0) > 0)
        {
            GetBallButton.interactable = true;
        }
        else
        {
            GetBallButton.interactable = false;
        }
    }


    private void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(CrystalsPack1.Id, ProductType.Consumable);
        builder.AddProduct(CrystalsPack2.Id, ProductType.Consumable);
        builder.AddProduct(RemoveAdsItem.Id, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }


    private void RemoveAds()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        m_StoreContoller.InitiatePurchase(RemoveAdsItem.Id);
    }

    private void Change()
    {
        if (PlayerPrefs.GetInt("Coins", 0) >= 1000)
        {
            if (Sound.SoundEnabled)
            {
                Sound.Tap.Play();
            }

            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) - 1000);
            PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) + 10);
            PlayerPrefs.Save();

            RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();
            CoinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString();

            if (PlayerPrefs.GetInt("Coins", 0) < 1000)
            {
                ChangeButton.interactable = false;
            }

            AdjustEvent coinsSpentEvent = new AdjustEvent("73kpji");
            Adjust.TrackEvent(coinsSpentEvent);
        }
    }

    private void GetBall()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        getFreeBall = true;
        AdsManager.Instance.rewardedAds.ShowRewardedAd();

        if (PlayerPrefs.GetInt("FreeBalls", 0) <= 1)
        {
            GetBallButton.interactable = false;
        }
    }

    private void BuyCrystalPack1()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        m_StoreContoller.InitiatePurchase(CrystalsPack1.Id);
    }

    private void BuyCrystalPack2()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        m_StoreContoller.InitiatePurchase(CrystalsPack2.Id);
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        print("Shop Initialized");
        m_StoreContoller = controller;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        print("Purchase Complete " + product.definition.id);

        if (product.definition.id == CrystalsPack1.Id)
        {
            PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) + 1000);
            PlayerPrefs.Save();

            RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();
        }

        else if (product.definition.id == CrystalsPack2.Id)
        {
            PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) + 5000);
            PlayerPrefs.Save();

            RubysText.text = PlayerPrefs.GetInt("Rubys", 0).ToString();
        }

        else if (product.definition.id == RemoveAdsItem.Id)
        {
            RemoveAdsButton.transform.parent.gameObject.SetActive(false);

            StartUI.adsOn = false;
            PlayerPrefs.SetInt("AdsOn", 0);
        }

        return PurchaseProcessingResult.Complete;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        print("Initialize failed " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        print("Initialize failed " + error + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        print("Purchase failed " + failureReason);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        print("Purchase failed " + failureDescription);
    }
}


[Serializable]
public class ConsumableItem
{
    public string Name;
    public string Id;
    public string desc;
    public float price;
}
[Serializable]
public class NonConsumableItem
{
    public string Name;
    public string Id;
    public string desc;
    public float price;
}