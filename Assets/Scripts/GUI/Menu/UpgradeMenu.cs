using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MenuBase<UpgradeMenu>
{
    [SerializeField] GameObject m_skillDescContainer;
    [SerializeField] Text m_priceText;
    [SerializeField] Image m_priceCurrency;
    [SerializeField] Button m_upgradeBtn;
    [SerializeField] Text m_confirmText;
    void Start()
    {
        ShowUpgradeUI(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoHome()
    {
        GUIManager.Instance.GotoHome();
    }

    public void OnShopButtonClick()
    {
        ShopMenu.Open();
    }

    public void OnAchivermentButtonClick()
    {
        AchivermentMenu.Open();
    }

    public void OnUpgradeButtonClick()
    {
        UpgradeMenu.Open();
    }

    public void LeaderboardButtonClick()
    {
        LeaderboardMenu.Open();
    }
    ////////////////////////////////////////////////////////////////////////////////
    GameObject currentDesc;
    int m_currentSelectSkill;
    public void OnSkillBtnClick(Transform button)
    {
        int idx = button.GetSiblingIndex();

        if (m_skillDescContainer)
        {
            if (idx <= m_skillDescContainer.transform.childCount)
            {
                var child = m_skillDescContainer.transform.GetChild(idx);
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                    currentDesc = null;

                    ShowUpgradeUI(false);

                    m_currentSelectSkill = -1;
                }
                else
                {
                    UpdateSkillDetail(idx);
                }
            }
        }
    }

    void UpdateSkillDetail(int idx)
    {
        var child = m_skillDescContainer.transform.GetChild(idx);

        if (currentDesc)
        {
            currentDesc.SetActive(false);
        }
        PassiveSkillInfo skillInfo = PassiveSkillManager.Instance.GetSkillInfo(idx);
        if (skillInfo != null)
        {
            currentDesc = child.gameObject;
            currentDesc.SetActive(true);

            m_currentSelectSkill = idx;

            var descText = child.GetComponentInChildren<Text>();
            int level = PlayerManager.Instance.GetSkillLevel(idx); //TODO: get current level of skill
            if (descText)
            {
                string text = string.Format(skillInfo.Desc, skillInfo.SkillValues[level],
                    level == skillInfo.MaxLevel ? skillInfo.SkillValues[level] : skillInfo.SkillValues[level + 1]);

                descText.text = text;

            }
            if (m_priceText)
            {
                if (level == skillInfo.MaxLevel)
                {
                    ShowUpgradeUI(false);
                    m_priceText.text = "MAX LEVEL";
                    if (m_priceText) m_priceText.gameObject.SetActive(true);

                }
                else
                {
                    m_priceText.text = skillInfo.SkillUpgradePrices[level].ToString();
                    ShowUpgradeUI(true);
                }
            }
        }
        else
        {
            currentDesc = null;
            ShowUpgradeUI(false);
        }
    }

    void ShowUpgradeUI(bool show)
    {
        if (m_upgradeBtn) m_upgradeBtn.gameObject.SetActive(show);
        if (m_priceText) m_priceText.gameObject.SetActive(show);
        if (m_priceCurrency) m_priceCurrency.gameObject.SetActive(show);
        if (m_confirmText) m_confirmText.gameObject.SetActive(!show);
    }
    public void OnConfirmBtnClick()
    {
        if (m_currentSelectSkill >= 0)
        {
            bool isWaitingConfirm = m_confirmText.gameObject.activeSelf;
            if (isWaitingConfirm)
            {
                if (m_confirmText) m_confirmText.gameObject.SetActive(false);
                bool ok = PlayerManager.Instance.UpgradeSkill(m_currentSelectSkill);
                if (ok)
                {
                    if (m_priceText) m_priceText.gameObject.SetActive(true);
                    if (m_confirmText) m_confirmText.gameObject.SetActive(false);

                    UpdateSkillDetail(m_currentSelectSkill);
                }
                else
                {
                    Debug.Log("Not enough money");
                    if (m_priceText) m_priceText.gameObject.SetActive(true);
                    if (m_confirmText) m_confirmText.gameObject.SetActive(false);
                }
            }
            else
            {
                //show confirm
                if (m_priceText) m_priceText.gameObject.SetActive(false);
                if (m_confirmText) m_confirmText.gameObject.SetActive(true);
            }
        }
    }
}
