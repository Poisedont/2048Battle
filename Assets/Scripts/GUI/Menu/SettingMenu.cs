using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingMenu : MenuBase<SettingMenu>
{

    [SerializeField] Slider m_soundVolume;
    [SerializeField] Toggle m_vibration;
    [SerializeField] Text m_FBText;
    // Start is called before the first frame update
    void Start()
    {
        m_soundVolume.value = SoundManager.Instance.SoundsVolume;
        if (PlayerManager.Instance.PlayerID != null)
        {
            FacebookManager.Instance.LoginFacebook();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (FB.IsLoggedIn)
        {
            m_FBText.text = "LOGOUT";
        }
        else
        {
            m_FBText.text = "LOGIN";
        }
    }

    public void OnAboutButtonCLick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        AboutMenu.Open();
    }

    public void OnSliderMove()
    {
        // keep the slider value for future use
        SoundManager.Instance.SoundsVolume = m_soundVolume.value;
        SoundManager.Instance.ChangeVolume();
    }

    public void LogInOutFB()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            PlayerManager.Instance.PlayerID = null;
            PlayerManager.Instance.SaveProfile();
        }
        else
        {
            FacebookManager.Instance.LoginFacebook();
        }
    }
}
