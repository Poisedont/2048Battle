using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Monetization;

public class AdsManager : Singleton<AdsManager>
{
    /// <summary>
    /// Unity Ads
    /// </summary>
    string gameId = "3344588";
    bool testMode = false;
    private string placementIdVideo = "rewardedVideo";

    private bool m_enableAds;

    UnityAction callback;

    private void Start()
    {

        // Initialize the Unity Ads SDK.
        Monetization.Initialize(gameId, testMode);
       // StartCoroutine(ShowInterstitialWhenReady());
    }

    private void Update()
    {
    }

    IEnumerator ShowVideoWhenReady()
    {
        while (!Monetization.IsReady(placementIdVideo))
        {
            //MonoBehaviour.print("wwwwwwwwwwadfgdfgdfhdfhdaaaaaaaafffff");
            yield return new WaitForSeconds(0.5f);
        }
        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementIdVideo) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show(AdFinished);
        }
        else
        {
            MonoBehaviour.print("Video unity ads is not ready yet");
        }
    }

    void AdFinished(ShowResult result)
    {
        if (result == ShowResult.Finished || result == ShowResult.Skipped)
        {
            // Reward the player
            //m_poincutEnterPLayCount = 0;
            //ResultMenu.DoubleGold();
            callback();
        }

    }

    public void ShowVideo(UnityAction function)
    {
        callback = function;
        StartCoroutine(ShowVideoWhenReady());
    }

    public void DisableAds()
    {
        Debug.Log("Ads disabled");
        m_enableAds = false;
        string abc = SystemInfo.deviceUniqueIdentifier.GetHashCode().ToString();
        PlayerPrefs.SetInt(abc, m_enableAds ? 1 : 0);
    }

    private void CheckAdsEnable()
    {
        string abc = SystemInfo.deviceUniqueIdentifier.GetHashCode().ToString();
        int a = PlayerPrefs.GetInt(abc, 1);
        m_enableAds = a == 1;
    }
}
