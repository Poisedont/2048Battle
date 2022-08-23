using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchivermentMenu : MenuBase<AchivermentMenu>
{
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
}
