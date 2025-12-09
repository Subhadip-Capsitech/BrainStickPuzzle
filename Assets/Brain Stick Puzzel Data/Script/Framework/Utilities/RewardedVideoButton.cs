using UnityEngine;
using GoogleMobileAds.Api;

public class RewardedVideoButton : MonoBehaviour
{
    public static RewardedVideoButton Instance;
    private RewardedAd rewardedAd;

    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test ID

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MobileAds.Initialize(init => LoadRewardedAd());
    }

    public bool IsReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public void ForceLoad()
    {
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        Debug.Log("Loading rewarded ad...");

        rewardedAd = null;  // reset

        var request = new AdRequest(); // no Builder()

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load rewarded ad: " + error);
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded ad loaded.");

            RegisterAdEvents(ad);
        });
    }

    private void RegisterAdEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed → Loading new one");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ad failed to show: " + error);
            LoadRewardedAd();
        };
    }

    public void OnClick()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                int amount = 2;
                GameManager.Hints += amount;
                Toast.instance.ShowMessage($"You received {amount} hints!");
            });
        }
        else
        {
            Toast.instance.ShowMessage("Ad loading... please wait.");
            LoadRewardedAd();
        }
    }
}
