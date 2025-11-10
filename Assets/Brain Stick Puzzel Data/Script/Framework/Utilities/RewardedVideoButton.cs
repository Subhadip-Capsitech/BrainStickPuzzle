using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class RewardedVideoButton : MonoBehaviour
{
    private RewardedAd rewardedAd;
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test Ad Unit ID

    private void Start()
    {
        MobileAds.Initialize(initStatus => { LoadRewardedAd(); });
    }

    private void LoadRewardedAd()
    {
        Debug.Log("Loading rewarded ad...");

        var adRequest = new AdRequest(); // ✅ No Builder() anymore

        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                rewardedAd = null;
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded ad loaded successfully.");

            RegisterEventHandlers(ad);
        });
    }

    public void OnClick()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                int amount = 2;
                GameManager.Hints += amount;
                Toast.instance.ShowMessage($"You've received {amount} hints!");
            });
        }
        else
        {
            Toast.instance.ShowMessage("Ad not ready, loading again...");
            LoadRewardedAd();
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ad closed. Loading a new one...");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ad failed to show: " + error);
            LoadRewardedAd();
        };

        ad.OnAdClicked += () => Debug.Log("Ad clicked");
        ad.OnAdImpressionRecorded += () => Debug.Log("Ad impression recorded");
        ad.OnAdPaid += (AdValue adValue) => Debug.Log("Ad paid: " + adValue.Value);
    }
}
