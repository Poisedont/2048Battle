using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenu : MenuBase<MainMenu>
{

    [SerializeField] Text m_score;
    [SerializeField] GameObject m_listMap;
    [SerializeField] Image m_mapSeletedImg;
    [SerializeField] Text m_mapSlectedName;

    [SerializeField] List<Sprite> m_listMapsSprite;
    [SerializeField] List<string> m_ListMapsName;

    private System.Action m_updateScoreFunction;
    // Start is called before the first frame update
    void Start()
    {
        GridManager.Instance.UpdateCurrentMap();
        m_updateScoreFunction += UpdateScore;

        GUIManager.Instance.AddScreentoStack(this);
        SoundManager.Instance.PlayMusic(SoundDefine.k_Music_Game_Menu_Looping);

        if (PlayerManager.Instance)
        {
            if (m_score) m_score.text = PlayerManager.Instance.Score.ToString();
        }

        for (int i = 0; i < SkillController.Instance.m_skills.Length; i++)
        {
            SkillController.Instance.m_skills[i].SetCount();
        }

        m_mapSeletedImg.sprite = m_listMapsSprite[PlayerManager.Instance.MapSeleted];
        m_mapSlectedName.text = m_ListMapsName[PlayerManager.Instance.MapSeleted];

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnMenuOpening()
    {
        base.OnMenuOpening();
        UpdateScore();
    }

    public override void OnBack()
    {
        //NO action
    }

    public void OnSettingButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        SettingMenu.Open();
    }

    public void OnPlayButtonClick()
    {
        //PlayerManager.Instance.AddScore(1, m_updateScoreFunction); ///Quyen test
        //PlayerManager.Instance.SaveProfile(); ///Quyen test

        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        GamePlayMenu.Open(); // Update in the future.
    }

    public void OnLBButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        LeaderboardMenu.Open();
    }

    public void OnShopButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        ShopMenu.Open();
    }

    public void OnLevelButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        UpgradeMenu.Open();
    }

    public void OnGiftButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        AchivermentMenu.Open();
    }

    private void UpdateScore()
    {
        m_score.text = PlayerManager.Instance.Score.ToString();
    }

    public void HideListMap()
    {
        m_listMap.gameObject.SetActive(false);
    }

    public void ShowListMap()
    {
        m_listMap.gameObject.SetActive(true);
    }

    public void SetMapName(string name)
    {
        m_mapSlectedName.text = name;
    }

    public void SetMapImg(Sprite sprite)
    {
        m_mapSeletedImg.sprite = sprite;
    }

    public void OnMapSeletedBtnClick()
    {
        if(m_listMap.gameObject.activeSelf)
        {
            HideListMap();
        }
        else
        {
            ShowListMap();
        }
    }
}
