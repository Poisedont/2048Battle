using UnityEngine;
using UnityEngine.UI;

public class ConfirmAdsMenu : MenuBase<ConfirmAdsMenu>
{
    [SerializeField] Text m_content;

    ////////////////////////////////////////////////////////////////////////////////
    public void OnNoBtnClick()
    {
        Close();

        GameplayController.Instance.ResumeGame();
    }

    public void OnYesBtnClick()
    {
        Close();
        // TODO: show ads instead of show reward
        SkillRewardMenu.Open();
    }
}