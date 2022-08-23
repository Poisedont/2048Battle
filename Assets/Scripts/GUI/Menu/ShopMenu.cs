using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MenuBase<ShopMenu>
{
    [SerializeField] List<Sprite> m_listTabIcon;
    [SerializeField] Image m_tabIcon;
    [SerializeField] GameObject m_popupNotEnough;
    [SerializeField] ScrollSnapRect m_scroll;

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void UpdateTabIcon()
    {
        m_tabIcon.sprite = m_listTabIcon[m_scroll.CurrentPage];
    }

    public void ShowHidePopup(bool pIsShow)
    {
        m_popupNotEnough.gameObject.SetActive(pIsShow);
    }

    public void GotoHCShop()
    {
        ShowHidePopup(false);
        m_scroll.MoveToPage(3);
    }
}
