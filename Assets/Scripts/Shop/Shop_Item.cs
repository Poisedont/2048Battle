using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum TYPE_PRICE
{
    ADS,
    SC,
    HC,
    IAP
}

public enum ITEM_ID
{
    CHEST1,
    CHEST2,
    CHEST3,
    CHEST4,
    BOOSTER_B1,
    BOOSTER_B2,
    BOOSTER_B3,
    BOOSTER_B4,
    SC1,
    SC2,
    SC3,
    SC4,
    HC1,
    HC2,
    HC3,
    HC4
}

public class Shop_Item : MonoBehaviour
{
    [SerializeField] ITEM_ID m_itemID;
    [SerializeField] Text m_itemName;
    [SerializeField] Text m_Decs;
    [SerializeField] SpriteMap m_currencyMap;
    [SerializeField] Image m_CurrencyImg;
    private int m_itemNumber;

    [SerializeField] Text m_itemPrice;

    [SerializeField] GameObject m_itemInfo;
    [SerializeField] ShopMenu m_shop;

    private ShopItemConfig itemInfo;


    UnityAction AdsCallBackFunction;
    // Start is called before the first frame update
    void Start()
    {
        itemInfo = ShopInfoDB.Instance.GetShopItemConfig(m_itemID.ToString());
        m_itemName.text = itemInfo.Name;
        m_Decs.text = itemInfo.DECS;
        m_CurrencyImg.sprite = m_currencyMap.GetSprite(itemInfo.Currency);
        m_itemNumber = itemInfo.Quantity;
        m_itemPrice.text = itemInfo.Price.ToString();
        string typePrice = itemInfo.Currency;

        if (typePrice.Equals(TYPE_PRICE.ADS.ToString()))
        {
            m_itemPrice.text = "WATCH AN AD";
        }

         AdsCallBackFunction += UpdateItemPurchare;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuyButtonCLick()
    {
        string typePrice = itemInfo.Currency;

        if(typePrice.Equals(TYPE_PRICE.ADS.ToString()))
        {
            //UpdateItemPurchare();
            AdsManager.Instance.ShowVideo(AdsCallBackFunction);
        }
        else if (typePrice.Equals(TYPE_PRICE.SC.ToString()))
        {
            if (PlayerManager.Instance.Soft_Currency >= itemInfo.Price)
            {
                PlayerManager.Instance.Soft_Currency -= (int)itemInfo.Price;
                UpdateItemPurchare();
            }
            else
            {
                m_shop.ShowHidePopup(true);
                ///Show Popup Not enough currrency
            }
        }
        else if (typePrice.Equals(TYPE_PRICE.HC.ToString()))
        {
            if (PlayerManager.Instance.Hard_Currency >= itemInfo.Price)
            {
                PlayerManager.Instance.Hard_Currency -= (int)itemInfo.Price;
                UpdateItemPurchare();
            }
            else
            {
                m_shop.ShowHidePopup(true);
                ///Show Popup Not enough currrency
            }
        }
        else if (typePrice.Equals(TYPE_PRICE.IAP.ToString()))
        {
            UpdateItemPurchare();
        }

    }

    public void OnInfoButtonClick()
    {
        m_itemInfo.gameObject.SetActive(!m_itemInfo.gameObject.activeSelf);
    }

    public void UpdateItemPurchare()
    {
        switch(m_itemID)
        {
            case ITEM_ID.CHEST1:
                ChestDB.Instance.m_chestSeleted = CHEST_TYPE.DAILY;
                OpenChestMenu.Open();
                //Call random
                break;
            case ITEM_ID.CHEST2:
                ChestDB.Instance.m_chestSeleted = CHEST_TYPE.COMMON;
                OpenChestMenu.Open();
                //Call random
                break;
            case ITEM_ID.CHEST3:
                ChestDB.Instance.m_chestSeleted = CHEST_TYPE.RARE;
                OpenChestMenu.Open();
                //Call random
                break;
            case ITEM_ID.CHEST4:
                ChestDB.Instance.m_chestSeleted = CHEST_TYPE.EPIC;
                OpenChestMenu.Open();
                //Call random
                break;
            case ITEM_ID.BOOSTER_B1:
                PlayerManager.Instance.Booster_B1 += m_itemNumber;
                break;
            case ITEM_ID.BOOSTER_B2:
                PlayerManager.Instance.Booster_B2 += m_itemNumber;
                break;
            case ITEM_ID.BOOSTER_B3:
                PlayerManager.Instance.Booster_B3 += m_itemNumber;
                break;
            case ITEM_ID.BOOSTER_B4:
                PlayerManager.Instance.Booster_B4 += m_itemNumber;
                break;
            case ITEM_ID.SC1:
                PlayerManager.Instance.Soft_Currency += m_itemNumber;
                break;
            case ITEM_ID.SC2:
                PlayerManager.Instance.Soft_Currency += m_itemNumber;
                break;
            case ITEM_ID.SC3:
                PlayerManager.Instance.Soft_Currency += m_itemNumber;
                break;
            case ITEM_ID.SC4:
                PlayerManager.Instance.Soft_Currency += m_itemNumber;
                break;
            case ITEM_ID.HC1:
                PlayerManager.Instance.Hard_Currency += m_itemNumber;
                break;
            case ITEM_ID.HC2:
                PlayerManager.Instance.Hard_Currency += m_itemNumber;
                break;
            case ITEM_ID.HC3:
                PlayerManager.Instance.Hard_Currency += m_itemNumber;
                break;
            case ITEM_ID.HC4:
                PlayerManager.Instance.Hard_Currency += m_itemNumber;
                break;
        }
        PlayerManager.Instance.SaveProfile();
        for (int i = 0; i < SkillController.Instance.m_skills.Length; i++)
        {
            SkillController.Instance.m_skills[i].SetCount();
        }
    }
}
