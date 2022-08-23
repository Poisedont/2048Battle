using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GGPManager : Singleton<GGPManager>
{

    // Start is called before the first frame update
    void Start()
    {
        // Create client configuration
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SignInGGP()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
    }

    public void SignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("(Lollygagger) Signed in!");
            PlayGamesPlatform.Instance.ReportScore(PlayerManager.Instance.Score,
                       GPGSIds.leaderboard_score,
                       (bool LBsuccess) =>
                       {

                           Debug.Log(" Leaderboard update success: " + LBsuccess);
                       });

        }
        else
        {
            Debug.Log("(Lollygagger) Sign-in failed...");

        }
    }

}

#endif