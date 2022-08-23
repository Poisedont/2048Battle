using UnityEngine;
using UnityEngine.UI;

public class UISkillPanel : MonoBehaviour
{
    [SerializeField] GameObject m_descObj;
    [SerializeField] Text m_descText;
    void Start()
    {
        if (m_descObj)
        {
            m_descObj.SetActive(false);
        }
    }

    private void Update()
    {
        if (GameplayController.Instance.SkillActivated)
        {
            if (m_descObj && !m_descObj.activeSelf)
            {
                m_descObj.SetActive(true);
            }
            if (m_descText)
            {
                Skill skill = SkillController.Instance.ActiveSkill;
                m_descText.text = skill.BoosterInfo.Desc;
            }
        }
        else
        {
            if (m_currentSkillInfo == null)
            {
                if (m_descObj && m_descObj.activeSelf)
                {
                    m_descObj.SetActive(false);
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    Skill m_currentSkillInfo;
    public void ShowSkillInfo(Skill skill)
    {
        if (m_currentSkillInfo == null || m_currentSkillInfo != skill)
        {
            m_currentSkillInfo = skill;
            if (m_descObj && !m_descObj.activeSelf)
            {
                m_descObj.SetActive(true);

            }
            if (m_descText)
            {
                m_descText.text = skill.BoosterInfo.Desc;
            }
        }
        else
        {
            m_currentSkillInfo = null;
            if (m_descObj)
            {
                m_descObj.SetActive(false);
            }
        }
    }
}
