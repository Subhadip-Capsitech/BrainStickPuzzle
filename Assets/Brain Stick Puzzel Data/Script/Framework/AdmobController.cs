using UnityEngine;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdmobController : MonoBehaviour
{
    public static AdmobController instance;

    [Header("Banner")]
    public string androidBanner;
    public string iosBanner;

    [Header("Interstitial")]
    public string androidInterstitial;
    public string iosInterstitial;

    [Header("Rewarded")]
    public string androidRewarded;
    public string iosRewarded;

    public int intersitialAdPeriod = 75;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

#if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true);
#endif

        if (!GameManager.IsAdRemoved)
        {
            RequestBanner();
            RequestInterstitial();
        }

        RequestRewardedAd();
    }

    // ----------------------------
    //  BANNER
    // ----------------------------
    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = androidBanner.Trim();
#elif UNITY_IOS
        string adUnitId = iosBanner.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += () => Debug.Log("Banner loaded");
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) => Debug.LogError("Banner failed: " + error.GetMessage());

        bannerView.LoadAd(CreateAdRequest());
    }

    public void ShowBanner() => bannerView?.Show();
    public void HideBanner() => bannerView?.Hide();

    // ----------------------------
    //  INTERSTITIAL
    // ----------------------------
    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = androidInterstitial.Trim();
#elif UNITY_IOS
        string adUnitId = iosInterstitial.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        InterstitialAd.Load(adUnitId, CreateAdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load interstitial: " + error);
                return;
            }

            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial closed");
                RequestInterstitial();
            };
        });
    }

    public void ShowInterstitial()
    {
        if (GameManager.IsAdRemoved) return;
        if (Time.time - PlayerPrefs.GetFloat("lastAdmobTime", -9999) < intersitialAdPeriod) return;

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            PlayerPrefs.SetFloat("lastAdmobTime", Time.time);
        }
        else
        {
            Debug.Log("Interstitial not ready — reloading...");
            RequestInterstitial();
        }
    }

    // ----------------------------
    //  REWARDED
    // ----------------------------
    public void RequestRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = androidRewarded.Trim();
#elif UNITY_IOS
        string adUnitId = iosRewarded.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        RewardedAd.Load(adUnitId, CreateAdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded failed to load: " + error);
                return;
            }

            rewardedAd = ad;

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded closed — reloading...");
                RequestRewardedAd();
            };
        });
    }

    public void ShowRewardedAd(Action onRewardEarned = null)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"User rewarded: {reward.Amount} {reward.Type}");
                onRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready yet — reloading...");
            RequestRewardedAd();
        }
    }

    private AdRequest CreateAdRequest()
    {
        AdRequest request = new AdRequest();
        return request;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            ShowInterstitial();
        }
    }

    public bool IsRewardedAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }
}
