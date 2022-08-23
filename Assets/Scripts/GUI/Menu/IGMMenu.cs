using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IGMMenu : MenuBase<IGMMenu>
{
    [SerializeField] GameObject m_ConirmPopup;
    [SerializeField] Text m_ConfirmGoMMText;
    [SerializeField] Text m_ConfirmReplayText;

    private 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnBack()
    {
        base.OnBack();
    }

    public void OnResumeBtnClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        Close();
        GameplayController.Instance.ResumeGame();
    }

    public void HomeBtnClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        m_ConfirmGoMMText.gameObject.SetActive(true);
        m_ConfirmReplayText.gameObject.SetActive(false);
        m_ConirmPopup.gameObject.SetActive(true);

       
    }

    public void ReplayBtnClick()
    {

        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        m_ConfirmGoMMText.gameObject.SetActive(false);
        m_ConfirmReplayText.gameObject.SetActive(true);
        m_ConirmPopup.gameObject.SetActive(true);

    }

    public void OnYesBtnPopupClick()
    {
        

        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        GameplayController.Instance.StopGame();
        GridManager.Instance.Clear();
        SkillController.Instance.Clear();

        if (m_ConfirmGoMMText.IsActive())
        {
            GUIManager.Instance.GotoHome();
        }
        else if (m_ConfirmReplayText.IsActive())
        {
            GameplayController.Instance.StartGame();
            GamePlayMenu.Open();
        }
        m_ConirmPopup.gameObject.SetActive(false);
    }

    public void OnNoBtnPopupClick()
    {
        m_ConirmPopup.gameObject.SetActive(false);
    }
}
