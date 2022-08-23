using UnityEngine;
using UnityEngine.UI;

public class SkillRewardMenu : MenuBase<SkillRewardMenu>
{
    [Tooltip("Object for Reward panel")]
    [SerializeField] GameObject m_selectBox;
    [SerializeField] GameObject m_skillReward;

    [SerializeField] Image m_skillIcon;
    ////////////////////////////////////////////////////////////////////////////////
    protected override void OnMenuOpening()
    {
        if (m_selectBox && !m_selectBox.activeSelf)
        {
            m_selectBox.SetActive(true);
        }
        if (m_skillReward && m_skillReward.activeSelf)
        {
            m_skillReward.SetActive(false);
        }
    }
    public void OnRewardBtnClick()
    {
        if (m_selectBox)
        {
            m_selectBox.SetActive(false);
        }
        if (m_skillReward)
        {
            m_skillReward.SetActive(true);

            int index = SkillController.Instance.RewardRandomSkill();
            Sprite sprite = SkillController.Instance.GetSkillSprite(index);

            if (m_skillIcon)
            {
                m_skillIcon.sprite = sprite;
            }
        }
    }

    public void OnSkillOKBtnClick()
    {
        Close();

        GameplayController.Instance.ResumeGame();
    }
}