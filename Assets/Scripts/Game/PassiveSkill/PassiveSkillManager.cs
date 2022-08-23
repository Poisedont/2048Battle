using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PassiveSkillManager : Singleton<PassiveSkillManager>
{
    [SerializeField] SkillData m_skillData;
    List<PassiveSkillInfo> m_skillInfos;

    const string k_passive_skill_file = "database/MMB_Skill";

    private void Start()
    {
        m_skillInfos = new List<PassiveSkillInfo>();
        LoadDB();

        LoadSkillData();
    }

    bool LoadDB()
    {
        TextAsset bin = Resources.Load(k_passive_skill_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("PassiveSkillManager: Can't load data " + k_passive_skill_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    PassiveSkillInfo skill = PassiveSkillInfo.Create(reader);
                    m_skillInfos.Add(skill);
                }
            }
        }
        return true;
    }

    public PassiveSkillInfo GetSkillInfo(string id)
    {
        return m_skillInfos.Find(skill => skill.ID.Equals(id));
    }
    public PassiveSkillInfo GetSkillInfo(int index)
    {
        if (index >= 0 && index < m_skillInfos.Count)
        {
            return m_skillInfos[index];
        }
        return null;
    }

    public void LoadSkillData()
    {
        if (m_skillData)
        {
            m_skillData.ClearData();
            for (int i = 0; i < m_skillInfos.Count; i++)
            {
                int skillLevel = 0; //TODO: get from saved 

                if (skillLevel < 0) continue;
                
                string skillID = m_skillInfos[i].ID;

                if (skillID.Equals("S1")) {
                    m_skillData.doubleEvolRatio = m_skillInfos[i].SkillValues[skillLevel];
                }
                else if (skillID.Equals("S2")) {
                    m_skillData.allyHigherRatio = m_skillInfos[i].SkillValues[skillLevel];
                }
                else if (skillID.Equals("S3")) {
                    m_skillData.bonusGold = m_skillInfos[i].SkillValues[skillLevel];
                }
                else if (skillID.Equals("S4")) {
                    m_skillData.bonusGoldPerKill = m_skillInfos[i].SkillValues[skillLevel];
                }
                else if (skillID.Equals("S5")) {
                    m_skillData.timePrepareAdd = m_skillInfos[i].SkillValues[skillLevel];
                }
                else if (skillID.Equals("S6")) {
                    m_skillData.turnBattleAdd = m_skillInfos[i].SkillValues[skillLevel];
                }
                else if (skillID.Equals("S7")) {
                    m_skillData.surviveRatioWhenAtkSameLevelUnit = m_skillInfos[i].SkillValues[skillLevel];
                }
                else if (skillID.Equals("S8")) {
                    m_skillData.blockEnemyPortalRatio = m_skillInfos[i].SkillValues[skillLevel];
                }
                else
                {
                    Debug.LogWarning("You have not implement effect for Skill " + skillID);
                }

            }
        }
    }

    public SkillData CurrentSkillData { get { return m_skillData; } }
}