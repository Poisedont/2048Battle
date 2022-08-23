using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using GooglePlayGames;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardMenu : MenuBase<LeaderboardMenu>
{

    [SerializeField] List<Sprite> m_listRankIcon;
    [SerializeField] GameObject[] stateLogin = new GameObject[3];

    [SerializeField] LeaderboardUITemp tempUI;
    private List<LeaderboardUITemp> listScoreUI;

    [SerializeField] Image m_avatar;
    [SerializeField] Text m_FBName;
    [SerializeField] Text m_score;
    [SerializeField] Text m_rank;
    [SerializeField] Image m_rankIcon;

    private Dictionary<LBSTATE, GameObject> m_listObject;

    private LBSTATE currentState = LBSTATE.LOGIN;

    private bool m_waitting_upload;

    [SerializeField] Button m_shareBtn;

    // Start is called before the first frame update
    void Start()
    {
        m_listObject = new Dictionary<LBSTATE, GameObject>();

        for (int i = 0; i < stateLogin.Length; i++)
        {
            m_listObject.Add((LBSTATE)i, stateLogin[i]);
        }

        m_waitting_upload = false;

        listScoreUI = new List<LeaderboardUITemp>();

        if (currentState == LBSTATE.LOGGED)
        {
            ChangeState(LBSTATE.UPLOAD);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case LBSTATE.LOGIN:

                m_shareBtn.gameObject.SetActive(false);
                if (PlayerManager.Instance.PlayerID != null)
                {
                    FacebookManager.Instance.IsLogged = 0;
                    FacebookManager.Instance.LoginFacebook();
                    ChangeState(LBSTATE.UPLOAD);
                }

                break;
            case LBSTATE.UPLOAD:
                if (FacebookManager.Instance.IsLogged == 1 && FacebookManager.Instance.GetUserName() != null && FacebookManager.Instance.GetUserAvatar() != null)
                {
                    UpdateScore();
                    ChangeState(LBSTATE.WAITTING_UPLOAD);
                }

                if (FacebookManager.Instance.IsLogged == -1 || FacebookManager.Instance.IsLogged == -2)
                {
                    ChangeState(LBSTATE.LOGIN);
                }
                break;
            case LBSTATE.WAITTING_UPLOAD:

                break;
            case LBSTATE.WAITTING_DOWNLOAD:

                if (FacebookManager.Instance.IsLogged == 1 && FacebookManager.Instance.GetUserName() != null && FacebookManager.Instance.GetUserAvatar() != null)
                {

                    SetupLeaderboard();

                    m_avatar.sprite = FacebookManager.Instance.GetUserAvatar();
                    m_FBName.text = FacebookManager.Instance.GetUserName();

                    ChangeState(LBSTATE.LOGGED);
                }

                if (FacebookManager.Instance.IsLogged == -1 || FacebookManager.Instance.IsLogged == -2)
                {
                    ChangeState(LBSTATE.LOGIN);
                }
                break;

            case LBSTATE.LOGGED:
                m_shareBtn.gameObject.SetActive(true);
                break;

        }
    }

    protected override void OnMenuOpening()
    {
        base.OnMenuOpening();
        if (currentState == LBSTATE.LOGGED || currentState == LBSTATE.UPLOAD || currentState == LBSTATE.WAITTING_UPLOAD || currentState == LBSTATE.WAITTING_DOWNLOAD)
        {
            ChangeState(LBSTATE.LOGIN);
        }
    }

    public void OnGlobalButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
 #if UNITY_ANDROID
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            GGPManager.Instance.SignInGGP();
        }
        else
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
#endif
    }


    public void OnFriendButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        LBFriendMenu.Open();
    }

    public void GoHome()
    {
        GUIManager.Instance.GotoHome();
    }

    public void OnShopButtonClick()
    {
        ShopMenu.Open();
    }

    public void OnAchivermentButtonClick()
    {
        AchivermentMenu.Open();
    }

    public void OnUpgradeButtonClick()
    {
        UpgradeMenu.Open();
    }

    public void LeaderboardButtonClick()
    {
        LeaderboardMenu.Open();
    }

    public void OnFBButtonCLick()
    {
        FacebookManager.Instance.IsLogged = 0;
        FacebookManager.Instance.LoginFacebook();
        ChangeState(LBSTATE.WAITTING_UPLOAD);

    }

    public void HideALlBody()
    {
        for (int i = 0; i < stateLogin.Length; i++)
        {
            stateLogin[i].SetActive(false);
        }
    }

    public void ChangeState(LBSTATE state)
    {
        HideALlBody();
        currentState = state;
        m_listObject[state].SetActive(true);
    }

    public void IsInternetConnected(Action<bool> action)
    {
        StartCoroutine(checkInternetConnection(action));
    }
    /// <summary>
    /// //////
    ///Example
    ///   
    /// StartCoroutine(checkInternetConnection((isConnected)=>{
    /// <handle connection status here> }));
    ///
    /// </summary>
    /// <returns>The internet connection.</returns>
    /// <param name="action">Action.</param>
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }


    public void UpdateScore()
    {
        StartCoroutine(checkUpdateScore());
    }

    IEnumerator checkUpdateScore()
    {

        string UploadScoreUrl = GameConst.k_LEADERBOARD_URL + GameConst.k_LEADERBOARD_PRIVATE_CODE + "/add/" + PlayerManager.Instance.PlayerID + "/" + PlayerManager.Instance.Score + "/0" + "/" + FacebookManager.Instance.GetUserName();
        WWW www = new WWW(UploadScoreUrl);

        yield return www;
        if (www.error != null)
        {
            ChangeState(LBSTATE.LOGIN);
        }
        else
        {
            ChangeState(LBSTATE.WAITTING_DOWNLOAD);
        }
    }

    public void SetupLeaderboard()
    {
        for (int k = 0; k < listScoreUI.Count; k++)
        {
            Destroy(listScoreUI[k].gameObject);
        }

        listScoreUI.Clear();

        List<FriendFacebook> listFriend = FacebookManager.Instance.GetFrienfList();

        string url = "http://dreamlo.com/lb/5d6739f0e6a81b07f016e034/pipe-get/";/// +ID Facebbok

        for (int i = 0; i < listFriend.Count; i++)
        {
            string profile = new WebClient().DownloadString(url + listFriend[i].m_id);

            string[] profileArray = profile.Split('|');

            Debug.Log("11 " + profileArray[0] + " " + profileArray[1]);

            int score = Int32.Parse(profileArray[1]);

            listFriend[i].SetScore(score);
        }

        QuickSort(listFriend, 0, listFriend.Count);

        for (int j = 0; j < listFriend.Count; j++)
        {
            LeaderboardUITemp element = Instantiate<LeaderboardUITemp>(tempUI, tempUI.transform.parent);

            string username = listFriend[j].m_name;
            string score = listFriend[j].m_score.ToString();

            element.SetName(username);
            element.SetScore(score);
            element.SetRank(j);
            string id = listFriend[j].m_id;
            StartCoroutine(SetAvatarUser(element, id));

            element.gameObject.SetActive(true);
            listScoreUI.Add(element);

            if (id.Equals(PlayerManager.Instance.PlayerID))
            {
                m_rank.text = (j + 1).ToString();

                int indexIcon = j;
                if (indexIcon >= 3)
                {
                    indexIcon = 3;
                }
                m_rankIcon.sprite = m_listRankIcon[indexIcon];
            }
        }


        m_score.text = PlayerManager.Instance.Score.ToString();

    }

    IEnumerator SetAvatarUser(LeaderboardUITemp element, string id)
    {
        string avatar_url = "https://graph.facebook.com/" + id + "/picture?type=small";
        WWW www = new WWW(avatar_url);
        yield return www;

        Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        //gameObject.GetComponent<Image>().sprite = sprite;
        element.SetAvatar(sprite);
    }

    IEnumerator SetNameUser(LeaderboardUITemp element, string pname)
    {
        string avatar_url = "https://graph.facebook.com/" + pname + "/picture?type=small";
        WWW www = new WWW(avatar_url);
        yield return www;

        Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        //gameObject.GetComponent<Image>().sprite = sprite;
        element.SetAvatar(sprite);
    }

    static void QuickSort(List<FriendFacebook> data, int left, int right)
    {
        int i = left - 1,
            j = right;

        while (true)
        {
            FriendFacebook d = data[left];
            do i++; while (data[i].m_score > d.m_score);
            do j--; while (data[j].m_score < d.m_score);

            if (i < j)
            {
                FriendFacebook tmp = data[i];
                data[i] = data[j];
                data[j] = tmp;
            }
            else
            {
                if (left < j) QuickSort(data, left, j);
                if (++j < right) QuickSort(data, j, right);
                return;
            }
        }
    }

    public void OnShareButtonCLick()
    {
        StartCoroutine(TakeSSAndShare());
    }

    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath).SetSubject("").SetText("").Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }
}
