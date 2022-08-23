using UnityEngine;
using UnityEngine.UI;

public class UISkillButton : MonoBehaviour
{
    [SerializeField] Image m_countDown;
    [SerializeField] Image m_skillIcon;
    [SerializeField] protected Text m_countTxt;
    [SerializeField] Image m_enableMask;


    Skill m_skill;
    private void Start()
    {
        m_skill = GetComponentInChildren<Skill>();

        if (m_skill)
        {
            m_skill.SkillValueChanged += OnSkillValueChangedHandle;
            if (m_skillIcon && m_skill.SkillIcon)
            {
                m_skillIcon.overrideSprite = m_skill.SkillIcon;
            }

            UpdateTextCount();
            UpdateEnableMask();
        }
    }

    private void OnSkillValueChangedHandle()
    {
        UpdateTextCount();
        UpdateEnableMask();
    }

    private void Update()
    {
        if (m_countDown)
        {
            if (m_countDown.fillAmount > 0 || m_skill.CurrentCoolDown > 0)
            {
                m_countDown.fillAmount = m_skill.CoolDownPercent;
            }
        }
    }

    private void UpdateTextCount()
    {
        if (m_countTxt)
        {
            m_countTxt.text = m_skill.AvailableCount.ToString();
        }
    }

    private void UpdateEnableMask()
    {
        // if (m_enableMask)
        // {
        //     m_enableMask.gameObject.SetActive(m_skill.AvailableCount <= 0);
        // }
    }
}