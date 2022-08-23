using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillController : Singleton<SkillController>
{
    private Skill m_activeSkill;

    public Skill ActiveSkill
    {
        get { return m_activeSkill; }
        set
        {
            m_activeSkill = value;

            GameplayController.Instance.SkillActivated = m_activeSkill != null;
        }
    }

    public Skill[] m_skills;

    ////////////////////////////////////////////////////////////////////////////////
    private List<BoosterInfo> m_boosters;

    const string k_booster_file = "database/MMB_Booster";

    public override void Awake()
    {
        base.Awake(); //bat buoc

        m_boosters = new List<BoosterInfo>();
        LoadBooster();
    }
    private void Start()
    {

    }

    public void OnStartGame()
    {
        // TODO: force add skill count for each skill
        for (int i = 0; i < m_skills.Length; i++)
        {
           m_skills[i].SetCount();
        }
    }
    /// <summary>
    /// Randomize an skill then increase its counter and return its index
    /// </summary>
    public int RewardRandomSkill()
    {
        int rand = Random.Range(0, m_skills.Length);
        m_skills[rand].IncreaseSkillCount();

        return rand;
    }

    public Sprite GetSkillSprite(int index)
    {
        if (index >= 0 && index < m_skills.Length)
        {
            if (m_skills[index] != null)
            {
                return m_skills[index].SkillIcon;
            }
        }
        return null;
    }

    public void Clear()
    {
        m_activeSkill = null;
        for (int i = 0; i < m_skills.Length; i++)
        {
            if (m_skills[i] != null)
            {
                m_skills[i].Reset();
            }
        }
    }

    bool LoadBooster()
    {
        TextAsset bin = Resources.Load(k_booster_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("SkillController: Can't load data " + k_booster_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    BoosterInfo info = BoosterInfo.Create(reader);

                    m_boosters.Add(info);
                }
            }
        }
        return true;
    }

    public BoosterInfo GetBoosterInfo(string boosterId)
    {
        return m_boosters.Find((a) => a.ID.Equals(boosterId));
    }
}
