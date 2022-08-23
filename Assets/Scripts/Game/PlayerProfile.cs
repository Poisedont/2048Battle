using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    public int m_score;
    public int m_softCurrency;
    public int m_hardCurrency;
    public string m_ID;
    public int m_boosterB1;
    public int m_boosterB2;
    public int m_boosterB3;
    public int m_boosterB4;
    public int m_bestWavePass;
    public int m_bestWavePass1;
    public int m_bestWavePass2;
    public int m_mapSeleted;

    public int[] m_skillLevels;

    public PlayerProfile(PlayerManager player)
    {
        m_score = player.Score;
        m_softCurrency = player.Soft_Currency;
        m_hardCurrency = player.Hard_Currency;
        m_ID = player.PlayerID;
        m_boosterB1 = player.Booster_B1;
        m_boosterB2 = player.Booster_B2;
        m_boosterB3 = player.Booster_B3;
        m_boosterB4 = player.Booster_B4;
        m_bestWavePass = player.BestWavePass;
        m_bestWavePass1 = player.BestWavePass1;
        m_bestWavePass2 = player.BestWavePass2;
        m_mapSeleted = player.MapSeleted;
        m_skillLevels = player.SkillLevels;
    }
}
