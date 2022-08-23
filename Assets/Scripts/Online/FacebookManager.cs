using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using Facebook.MiniJSON;

public class FacebookManager : Singleton<FacebookManager>
{
    private string m_userName;
    private string m_userID;
    private Sprite m_avatar;
    private List<FriendFacebook> m_friendList = new List<FriendFacebook>();

    public int IsLogged = 0;
    public FacebookManager()
    {
    }

    public void Start()
    {
        if(PlayerManager.Instance.PlayerID != null)
        {
            LoginFacebook();
        }
    }

    public void LoginFacebook()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(IsInitCompleted, OnHideUnity);
        }
        else
        {
            IsInitCompleted();
        }
    }

    private void IsInitCompleted()
    {
        Debug.Log("FB init complete with " + FB.AppId);
        if (FB.IsLoggedIn)
        {
            SetNameAndAvatar(FB.IsLoggedIn);
        }
        else 
        {
            CallFBLogin();
        }

    }

    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Is game shown: " + isGameShown);
    }

    private void CallFBLogin()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, FBLoginCallback);
    }

    private void FBLoginCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
            IsLogged = -1;
        }
        SetNameAndAvatar(FB.IsLoggedIn);
    }

    void SetNameAndAvatar(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
            FB.API("/me?fields=id", HttpMethod.GET, SetFacebookUserID);
            FB.API("/me?fields=friends", HttpMethod.GET, SetFriendList);
        }
    }

    void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
            m_userName = "" + result.ResultDictionary["first_name"];

            Debug.Log("" + name);
        }
        else
        {
            Debug.Log(result.Error);
            IsLogged = -2;
        }
    }

    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            Debug.Log("Profile Pic");
            m_avatar = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
            IsLogged = 1;
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    void SetFacebookUserID(IGraphResult result)
    {
        if (result.Texture != null)
        {
            m_userID = "" + result.ResultDictionary["id"];

            if(!(m_userID.Equals(PlayerManager.Instance.PlayerID)))
            {
                PlayerManager.Instance.UpdatePlayerID(m_userID);
                PlayerManager.Instance.SaveProfile();
            }
        }
    }

    void SetFriendList(IGraphResult result)
    {
        if (result.Texture != null)
        {

            m_friendList.Clear();
            //Dictionary<string, Object> friends = new Dictionary<string, Object>();
            Dictionary<string, object> dict = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

            Dictionary<string, object> friends = (Dictionary<string, object>)dict["friends"];

            var m_List = new List<object>();
            m_List = (List<object>)(friends["data"]);

            int _friendCount = m_List.Count;
            Debug.Log("Items found:" + _friendCount);

            for(int i = 0; i< _friendCount; i++)
            {
                Dictionary<string, object> item = (Dictionary<string, object>)m_List[i];
                string id = item["id"].ToString();
                string name = item["name"].ToString();
                Debug.Log("Items ID:" + item["id"]);
                m_friendList.Add(new FriendFacebook(name, id));
            }

            m_friendList.Add(new FriendFacebook(m_userName, m_userID));
            //List<string> friendIDsFromFB = new List<string>();
            /*for (int i = 0; i < _friendCount; i++) // Tried this, same error.
            {

                foreach(KeyValuePair<string, object> entry in friendList)
                {
                    Debug.Log(entry.Key + "|" + entry.Value);
                }

                 string friendFBID = getDataValueForKey((Dictionary<string, object>)(friendList[i]), "id");
                string friendName = getDataValueForKey((Dictionary<string, object>)(friendList[i]), "name");

                Debug.Log(i + "/" + _friendCount + "|" + friendFBID +"|"+ friendName);
                NPBinding.UI.ShowToast(i + "/" + _friendCount + "|" + friendFBID + "|" + friendName, VoxelBusters.NativePlugins.eToastMessageLength.LONG);

                //friendIDsFromFB.Add(friendFBID);
            }*/

            //foreach (KeyValuePair<string, object> entry in friendList) // Tried this, same error.
            //{
            //    Debug.Log(entry.Key + "|" + entry.Value);
            //}

            // Debug.Log("friends:" + friends.ToString());
        }
    }

    public void LogOutFacebook()
    {
        if(FB.IsLoggedIn)
        {
            FB.LogOut();
            IsLogged = 0;
        }
    }

    public string GetUserName()
    {
        return m_userName;
    }

    public Sprite GetUserAvatar()
    {
        return m_avatar;
    }

    public List<FriendFacebook> GetFrienfList()
    {
        return m_friendList;
    }

}
