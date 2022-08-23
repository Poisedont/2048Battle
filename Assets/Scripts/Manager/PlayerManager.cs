using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : Singleton<PlayerManager>
{
    public GameObject m_topInfo;
    public int Score { set; get; }
    public int Soft_Currency { get; set; }
    public int Hard_Currency { get; set; }
    public string PlayerID { get; set; }

    public int Booster_B1 { get; set; }
    public int Booster_B2 { get; set; }
    public int Booster_B3 { get; set; }
    public int Booster_B4 { get; set; }

    public int BestWavePass { get; set; }
    public int BestWavePass1 { get; set; }
    public int BestWavePass2 { get; set; }
    public int MapSeleted { get; set; }
    /// <summary>
    /// No Save in player Profile 
    /// </summary>
    public int CurrentScore { get; set; }
    public int CurrentWavePass { get; set; }
    public int CurrentLoot { get; set; }
    public int CurrentKillEnemies { get; set; }
    public int CurrentGold { get; set; }

    public int[] SkillLevels { get; set; }

    private void Start()
    {
        LoadProfile();
    }

    public void SaveProfile()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadProfile()
    {
        PlayerProfile player = SaveSystem.LoadPlayer();

        if (player != null)
        {
            Score = player.m_score;
            Soft_Currency = player.m_softCurrency;
            Hard_Currency = player.m_hardCurrency;
            PlayerID = player.m_ID;
            Booster_B1 = player.m_boosterB1;
            Booster_B2 = player.m_boosterB2;
            Booster_B3 = player.m_boosterB3;
            Booster_B4 = player.m_boosterB4;
            BestWavePass = player.m_bestWavePass;
            BestWavePass1 = player.m_bestWavePass1;
            BestWavePass2 = player.m_bestWavePass2;
            MapSeleted = player.m_mapSeleted;

            SkillLevels = player.m_skillLevels;
            if (SkillLevels == null)
            {
                SkillLevels = new int[9];
            }
        }
        else
        {
            Score = 0;
            Soft_Currency = 0;
            Hard_Currency = 0;
            PlayerID = null;
            Booster_B1 = 0;
            Booster_B2 = 0;
            Booster_B3 = 0;
            Booster_B4 = 0;
            BestWavePass = 0;
            BestWavePass1 = 0;
            BestWavePass2 = 0;
            MapSeleted = 0;

            SkillLevels = new int[9];
        }
    }

    //public void AddScore(int pScore, System.Action action)
    //{
    //    Score += pScore;
    //    action();
    //}

    //public void AddCurrency(int pCurrency, System.Action action)
    //{
    //    Currency += pCurrency;
    //    action();
    //}

    public void UpdatePlayerID(string id)
    {
        PlayerID = id;
    }

    public bool UpgradeSkill(int idx)
    {
        if (idx >= 0 && idx < SkillLevels.Length)
        {
            PassiveSkillInfo info = PassiveSkillManager.Instance.GetSkillInfo(idx);
            if (info != null)
            {
                int curLv = SkillLevels[idx];
                float price = info.SkillUpgradePrices[curLv];
                if (price <= Soft_Currency)
                {
                    SkillLevels[idx]++; //level up
                    Soft_Currency -= (int)price; //cost money

                    SaveProfile();

                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Return level of passive skill idx (count from 1)
    /// 0 : skill not unlock
    /// > 0: skill level
    /// </summary>
    public int GetSkillLevel(int idx)
    {
        if (idx >= 0 && idx < SkillLevels.Length)
        {
            return SkillLevels[idx];
        }
        return 0;
    }
}
