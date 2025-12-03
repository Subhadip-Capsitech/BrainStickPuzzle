using UnityEngine;
using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

public class AdmobController : MonoBehaviour
{
    public static AdmobController instance;

    [Header("Banner")]
    string androidBanner = "ca-app-pub-8530302013109448/3323670511";
    public string iosBanner;

    [Header("Interstitial")]
    public string androidInterstitial;
    public string iosInterstitial;

    [Header("Rewarded")]
    public string androidRewarded;
    public string iosRewarded;

    [Header("Settings")]
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
            return;
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

        RequestBanner();
        RequestInterstitial();
        RequestRewardedAd();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // BANNER
    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = androidBanner;
#elif UNITY_IOS
        string adUnitId = iosBanner.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner loaded");
            bannerView.Show();
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner load failed: " + error.GetMessage());
        };

        bannerView.LoadAd(new AdRequest());
    }

    public void ShowBanner() => bannerView?.Show();
    public void HideBanner() => bannerView?.Hide();

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RecreateBannerDelayed());
    }

    private IEnumerator RecreateBannerDelayed()
    {
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.25f);
        RequestBanner();
    }

    // INTERSTITIAL
    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = androidInterstitial.Trim();
#elif UNITY_IOS
        string adUnitId = iosInterstitial.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        InterstitialAd.Load(adUnitId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Interstitial load failed: " + error);
                return;
            }

            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                RequestInterstitial();
            };
        });
    }

    // UPDATED SHOW INTERSTITIAL WITH TIMER + REMOVE ADS CHECK
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

    // REWARDED
    public void RequestRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = androidRewarded.Trim();
#elif UNITY_IOS
        string adUnitId = iosRewarded.Trim();
#else
        string adUnitId = "unexpected_platform";
#endif

        RewardedAd.Load(adUnitId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Rewarded load failed: " + error);
                return;
            }

            rewardedAd = ad;

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
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
                onRewardEarned?.Invoke();
            });
        }
        else
        {
            RequestRewardedAd();
        }
    }
}
