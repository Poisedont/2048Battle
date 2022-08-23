using System.Collections;
using System.Collections.Generic;
using System.IO;
using rds;
using UnityEngine;
using UnityEngine.UI;

public class OpenChestMenu : MenuBase<OpenChestMenu>
{

    [SerializeField] DropItemUI m_dropItemUITemp;
    private List<DropItemUI> m_listItem = new List<DropItemUI>();
    [SerializeField] Button m_backBtn;
    [SerializeField] Animator m_animator;
    [SerializeField] Image m_chestImage;

    [SerializeField] SpriteMap m_itemMap;
    [SerializeField] SpriteMap m_chestMap;

    List<DropItem> m_listDropItem;

    private Dictionary<string, int> m_sumResult;
    // Start is called before the first frame update
    void Start()
    {

    }

    protected override void OnMenuOpening()
    {
        base.OnMenuOpening();

        m_sumResult = new Dictionary<string, int>();
        for (int i = 0; i< m_listItem.Count; i++)
        {
            Destroy(m_listItem[i].gameObject);
        }
        m_listItem.Clear();

        m_chestImage.sprite = m_chestMap.GetSprite(ChestDB.Instance.m_chestSeleted.ToString());
        m_listDropItem = ChestDB.Instance.GetListDropItemSeleted();
        GenerateRandomItem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnBack()
    {
        if(m_backBtn.gameObject.activeSelf)
        {
            base.OnBack();
        }
    }

    public void PlayAnimOpenChest()
    {
        m_animator.Play("ShowItemFromChest", -1, 0f);
    }

    private void GenerateRandomItem()
    {
        RDSTable t = new RDSTable();

        RDSTable sc = new RDSTable();
        RDSTable hc = new RDSTable();
        RDSTable booter = new RDSTable();

        t.AddEntry(sc, GetPercentageGroup(ChestDB.k_SC_GROUP_NAME));
        t.AddEntry(hc, GetPercentageGroup(ChestDB.k_HC_GROUP_NAME));
        t.AddEntry(booter, GetPercentageGroup(ChestDB.k_BOOTER_GROUP_NAME));

        //heroPiece.rdsUnique = true;
        //expBook.rdsUnique = true;
        int minItem = 0;
        int maxItem = 0;

        switch (ChestDB.Instance.m_chestSeleted)
        {
            case CHEST_TYPE.DAILY:
                //sc.rdsAlways = true;
                hc.rdsUnique = true;
                booter.rdsUnique = true;
                minItem = 1;
                maxItem = 3;
                break;
            case CHEST_TYPE.COMMON:
               // sc.rdsAlways = true;
                booter.rdsAlways = true;
                minItem = 2;
                maxItem = 4;
                break;
            case CHEST_TYPE.RARE:
                //sc.rdsAlways = true;
                booter.rdsAlways = true;
                booter.rdsCount = 2;
                minItem = 2;
                maxItem = 4;
                break;
            case CHEST_TYPE.EPIC:
                sc.rdsAlways = true;
                hc.rdsAlways = true;
                booter.rdsAlways = true;
                booter.rdsCount = 4;
                minItem = 6;
                maxItem = 6;
                break;
        }


        for (int i = 0; i < m_listDropItem.Count; i++)
        {

            switch (m_listDropItem[i].m_groupName)
            {

                case ChestDB.k_SC_GROUP_NAME:
                    sc.AddEntry(m_listDropItem[i], m_listDropItem[i].m_percentage);
                    break;

                case ChestDB.k_HC_GROUP_NAME:
                    hc.AddEntry(m_listDropItem[i], m_listDropItem[i].m_percentage);
                    break;

                case ChestDB.k_BOOTER_GROUP_NAME:
                    booter.AddEntry(m_listDropItem[i], m_listDropItem[i].m_percentage);
                    break;
            }
        }


        int numberItemDrop = Random.Range(minItem, maxItem + 1);
        t.rdsCount = numberItemDrop;
        // Debug.Log("Run :" + i);
        //WriteLogBOX("Chest: ");
        foreach (DropItem m in t.rdsResult)
        {
            // Debug.Log("Item :" + m);
            //WriteLogBOX("Item Droped: " + m);
            //AddItem(m);
            //DropItemUI item = Instantiate<DropItemUI>(m_dropItemUITemp, m_dropItemUITemp.transform.parent);
            //item.gameObject.SetActive(true);
            //m_listItem.Add(item);


            switch (m.m_groupName)
            {
                case ChestDB.k_SC_GROUP_NAME:
                    int num = Random.Range(m.m_minDrop, m.m_maxDrop + 1);
                    UpdateItemToSumResult(m.m_groupName, num);
                   // item.UpdateIcon(m_itemMap.GetSprite(m.m_groupName));
                    break;
                case ChestDB.k_HC_GROUP_NAME:
                    int num1 = Random.Range(m.m_minDrop, m.m_maxDrop + 1);
                    UpdateItemToSumResult(m.m_groupName, num1);
                    //  item.UpdateIcon(m_itemMap.GetSprite(m.m_groupName));
                    break;
                case ChestDB.k_BOOTER_GROUP_NAME:
                    int num2 = Random.Range(m.m_minDrop, m.m_maxDrop + 1);
                    UpdateItemToSumResult(m.m_name, num2);
                    // item.UpdateIcon(m_itemMap.GetSprite(m.m_name));
                    break;
            }

        }

        

        if (m_sumResult.ContainsKey("SC"))
        {
            DropItemUI item = Instantiate<DropItemUI>(m_dropItemUITemp, m_dropItemUITemp.transform.parent);
            item.gameObject.SetActive(true);
            m_listItem.Add(item);

            item.UpdateIcon(m_itemMap.GetSprite("SC"));
            item.UpdateNumber(m_sumResult["SC"]);
            PlayerManager.Instance.Soft_Currency += m_sumResult["SC"];
        }

        if (m_sumResult.ContainsKey("HC"))
        {
            DropItemUI item = Instantiate<DropItemUI>(m_dropItemUITemp, m_dropItemUITemp.transform.parent);
            item.gameObject.SetActive(true);
            m_listItem.Add(item);

            item.UpdateIcon(m_itemMap.GetSprite("HC"));
            item.UpdateNumber(m_sumResult["HC"]);
            PlayerManager.Instance.Hard_Currency += m_sumResult["HC"];
        }

        if (m_sumResult.ContainsKey("Booster_B1"))
        {
            DropItemUI item = Instantiate<DropItemUI>(m_dropItemUITemp, m_dropItemUITemp.transform.parent);
            item.gameObject.SetActive(true);
            m_listItem.Add(item);

            item.UpdateIcon(m_itemMap.GetSprite("Booster_B1"));
            item.UpdateNumber(m_sumResult["Booster_B1"]);
            PlayerManager.Instance.Booster_B1 += m_sumResult["Booster_B1"];
        }

        if (m_sumResult.ContainsKey("Booster_B2"))
        {
            DropItemUI item = Instantiate<DropItemUI>(m_dropItemUITemp, m_dropItemUITemp.transform.parent);
            item.gameObject.SetActive(true);
            m_listItem.Add(item);

            item.UpdateIcon(m_itemMap.GetSprite("Booster_B2"));
            item.UpdateNumber(m_sumResult["Booster_B2"]);
            PlayerManager.Instance.Booster_B2 += m_sumResult["Booster_B2"];
        }

        if (m_sumResult.ContainsKey("Booster_B3"))
        {
            DropItemUI item = Instantiate<DropItemUI>(m_dropItemUITemp, m_dropItemUITemp.transform.parent);
            item.gameObject.SetActive(true);
            m_listItem.Add(item);

            item.UpdateIcon(m_itemMap.GetSprite("Booster_B3"));
            item.UpdateNumber(m_sumResult["Booster_B3"]);
            PlayerManager.Instance.Booster_B3 += m_sumResult["Booster_B3"];
        }

        if (m_sumResult.ContainsKey("Booster_B4"))
        {
            DropItemUI item = Instantiate<DropItemUI>(m_dropItemUITemp, m_dropItemUITemp.transform.parent);
            item.gameObject.SetActive(true);
            m_listItem.Add(item);

            item.UpdateIcon(m_itemMap.GetSprite("Booster_B4"));
            item.UpdateNumber(m_sumResult["Booster_B4"]);
            PlayerManager.Instance.Booster_B4 += m_sumResult["Booster_B4"];
        }


        //Save all item from Chest Box
        PlayerManager.Instance.SaveProfile();
        for (int i = 0; i < SkillController.Instance.m_skills.Length; i++)
        {
            SkillController.Instance.m_skills[i].SetCount();
        }
    }

    public double GetPercentageGroup(string groupName)
    {
        for (int i = 0; i < m_listDropItem.Count; i++)
        {
            if (m_listDropItem[i].m_groupName.Equals(groupName))
            {
                return m_listDropItem[i].m_percentageGroup;
            }
        }

        return 0;
    }

    public void WriteLogBOX(string item)
    {
#if UNITY_EDITOR
        using (StreamWriter writer = new StreamWriter("ChestLog", true))
        {
            writer.WriteLine(item);
        }
#endif
    }

    public void UpdateItemToSumResult(string itemName, int number )
    {
        if(m_sumResult.ContainsKey(itemName))
        {
            m_sumResult[itemName] += number;
        }
        else
        {
            m_sumResult.Add(itemName, number);
        }
    }
}
