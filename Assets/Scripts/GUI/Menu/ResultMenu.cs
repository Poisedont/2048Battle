using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResultMenu : MenuBase<ResultMenu>
{
    [SerializeField] Text m_title;

    [SerializeField] Text m_numPassWave;
    [SerializeField] Text m_numKillEnemies;
    [SerializeField] Text m_numLootGold;
    [SerializeField] Text m_numScore;
    [SerializeField] Text m_Timecount;
    [SerializeField] Text m_numDoubleLoot;
    [SerializeField] Button m_AdsideoBtn;

    private const float k_maxTime = 10;
    TimerCount m_time = new TimerCount();

    UnityAction callback;

    // Start is called before the first frame update
    void Start()
    {
        callback += DoubleGold;
    }

    protected override void OnMenuOpening()
    {
        m_title.text = ResultDB.Instance.GetResultStrConfig(PlayerManager.Instance.CurrentWavePass);

        m_time.Reset();
        m_time.Play();
        base.OnMenuOpening();
        m_Timecount.gameObject.SetActive(true);
        m_AdsideoBtn.interactable = true;
        m_numScore.text = PlayerManager.Instance.CurrentScore.ToString();
        int goldBonus = (int)PassiveSkillManager.Instance.CurrentSkillData.bonusGold;

        m_numLootGold.text = PlayerManager.Instance.CurrentGold.ToString() + (goldBonus > 0 ? " + " + goldBonus : "");
        m_numDoubleLoot.text = (PlayerManager.Instance.CurrentGold * 2).ToString();
        m_numKillEnemies.text = PlayerManager.Instance.CurrentKillEnemies.ToString();
        m_numPassWave.text = PlayerManager.Instance.CurrentWavePass.ToString() ;
    }

    protected override void OnMenuClosing()
    {
        base.OnMenuClosing();
        int goldBonus = (int)PassiveSkillManager.Instance.CurrentSkillData.bonusGold;
        PlayerManager.Instance.Soft_Currency += PlayerManager.Instance.CurrentGold + goldBonus;

        PlayerManager.Instance.SaveProfile();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Timecount.gameObject.activeSelf)
        {
            m_time.Update();
            float CurrenceTimeCount = m_time.ElapsedTime;
            int timeRemain = Convert.ToInt32(k_maxTime - CurrenceTimeCount);
            if (timeRemain >= 0)
            {
                m_Timecount.text = "TIME LEFT: " + timeRemain + "S";
            }
            else
            {
                EndTime();
            }
        }

        
    }

    public override void OnBack()
    {
        
    }

    private void EndTime()
    {

        m_Timecount.gameObject.SetActive(false);
        m_AdsideoBtn.interactable = false;
        m_time.Pause();
        m_time.Reset();
    }

    public void OnHomeButtonClick()
    {
        GameplayController.Instance.StopGame();
        GUIManager.Instance.GotoHome();
    }

    public void OnReplayButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        GridManager.Instance.Clear();
        SkillController.Instance.Clear();
        GameplayController.Instance.StartGame();
        GamePlayMenu.Open();
    }

    public void OnShareButtonClick()
    {
        SoundManager.PlaySFX(SoundDefine.k_SFX_ButtonClickSFXName);
        StartCoroutine(TakeSSAndShare());
    }

    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "sharedResultImg.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath).SetSubject("").SetText("").Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }

    public void PlayVideo()
    {
        m_time.Pause();
        AdsManager.Instance.ShowVideo(callback);
    }

    public void DoubleGold()
    {
        PlayerManager.Instance.CurrentGold *= 2;
        int goldBonus = (int)PassiveSkillManager.Instance.CurrentSkillData.bonusGold;
        m_numLootGold.text = PlayerManager.Instance.CurrentGold.ToString() + (goldBonus > 0 ? " + " + goldBonus : "");
        EndTime();
    }
}
