using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class LBGlobalMenu : MenuBase<LBGlobalMenu>
{
    [SerializeField] GameObject[] stateLogin = new GameObject[3];

    [SerializeField] LeaderboardUITemp tempUI;
    private List<LeaderboardUITemp> listScoreUI;

    [SerializeField] Image m_avatar;
    [SerializeField] Text m_FBName;
    [SerializeField] Text m_score;
    [SerializeField] Text m_rank;

    private Dictionary<LBSTATE, GameObject> m_listObject;

    private LBSTATE currentState = LBSTATE.LOGIN;

    private bool m_waitting_upload;
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
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case LBSTATE.LOGIN:

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

                break;

        }
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
        StartCoroutine(CheckInternetConnection(action));
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
    IEnumerator CheckInternetConnection(Action<bool> action)
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
        string ListScoreUrl = GameConst.k_LEADERBOARD_URL + GameConst.k_LEADERBOARD_PUBLIC_CODE + "/xml";

        var xml = new WebClient().DownloadString(ListScoreUrl);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        XmlNodeList list = xmlDoc.GetElementsByTagName("entry");

        for (int i = 0; i < list.Count; i++)
        {
            XmlDocument xmlElement = new XmlDocument();
            xmlElement.LoadXml(list.Item(i).OuterXml);

            LeaderboardUITemp element = Instantiate<LeaderboardUITemp>(tempUI, tempUI.transform.parent);


            string username = xmlElement.GetElementsByTagName("text").Item(0).InnerText;
            element.SetName(username);
            element.SetScore(xmlElement.GetElementsByTagName("score").Item(0).InnerText);

            string id = xmlElement.GetElementsByTagName("name").Item(0).InnerText;
            StartCoroutine(SetAvatarUser(element, id));

            element.gameObject.SetActive(true);
            listScoreUI.Add(element);
        }



        Debug.Log(xml.ToString());

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

}
