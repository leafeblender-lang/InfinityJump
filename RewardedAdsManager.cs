using UnityEngine;
using GoogleMobileAds.Api;

using System;
using System.Collections;

public class RewardedAdsManager : MonoBehaviour
{
    private RewardedAd rewardedAd;
    private Boolean reklamauToku = false;
#if UNITY_ANDROID
    private string adUnitId = "ca-app-pub-1222803386483169/6274045488";
#else
    private string adUnitId = "unused";
#endif
    [SerializeField] private GameObject bouncingBall;
    void Start()
    {
        // MobileAds.Initialize(initStatus =>
        // {
        //Debug.Log("Google Mobile Ads SDK initialized.");
        LoadRewardedAd();
        // });
    }

    public void LoadRewardedAd()
    {
        // Uništi stari oglas ako postoji
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading rewarded ad...");

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load rewarded ad: " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded successfully.");
            rewardedAd = ad;

            RegisterEventHandlers(rewardedAd);
        });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad impression recorded.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad clicked.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed.");
            LoadRewardedAd();  // Reload ad for next use
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + error);
            LoadRewardedAd();  // Reload ad even on failure
        };
    }
    

    public void ShowRewardedAd(int choose)
    {
        if (reklamauToku) return;
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            reklamauToku = true;
            rewardedAd.Show((Reward reward) =>
            {
                // Debug.Log($"User earned reward: {reward.Type}, amount: {reward.Amount}");
                switch (choose)
                {
                    case 1:
                        bouncingBall.GetComponent<BouncingBall>().odgledaoVideoZaPonovnoIgranje();
                        break;
                    case 2:
                        bouncingBall.GetComponent<BouncingBall>().odgledaoVideoZaDupliranje();
                        break;
                    case 3:
                        {
                            var gm = FindObjectOfType<GameManager_script>();
                            if (gm != null) gm.GrantLifeFromAd();
                        }
                        break;
                }
                reklamauToku = false; // Dozvoli sledeću reklamu
            });
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }
    public void showReclama(int i)
    {
        switch (i)
        {
            case 1:
                bouncingBall.GetComponent<BouncingBall>().odgledaoVideoZaPonovnoIgranje();
                break;
            case 2:
                bouncingBall.GetComponent<BouncingBall>().odgledaoVideoZaDupliranje();
                break;
            case 3:
                {
                    var gm = FindObjectOfType<GameManager_script>();
                    if (gm != null) gm.GrantLifeFromAd();
                }
                break;
        }
    }
    private void OnDestroy()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }
}
/*
    public void ShowRewardedAd(int choose)
    {
        if (reklamauToku) return;
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            reklamauToku = true;
            switch (choose)
            {
                case 1:
                    break;
                case 2:
                    break;
            }
            StartCoroutine(SimulirajReklamu(choose));


        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }

    }

    private IEnumerator SimulirajReklamu(int choose)
    {
        Debug.Log("Simulacija reklame počela...");

        // Pauziraj igru tokom "reklame" (opciono)
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(5f); // koristi Realtime da radi i kad je pauza

        Debug.Log("Simulacija reklame završena.");
        reklamauToku = false;
        Time.timeScale = 1f; // nastavi igru
        switch (choose)
        {
            case 1:
                bouncingBall.GetComponent<BouncingBall>().odgledaoVideoZaPonovnoIgranje();
                break;
            case 2:
                bouncingBall.GetComponent<BouncingBall>().odgledaoVideoZaDupliranje();
                break;


        }
    }
}
*/