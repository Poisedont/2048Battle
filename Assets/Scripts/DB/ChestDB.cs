using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum CHEST_TYPE
{
    DAILY,
    COMMON,
    RARE,
    EPIC
}



public class ChestDB :Singleton<ChestDB>
{
    public const string k_SC_GROUP_NAME = "SC";
    public const string k_HC_GROUP_NAME = "HC";
    public const string k_BOOTER_GROUP_NAME = "Booster";

    public CHEST_TYPE m_chestSeleted = CHEST_TYPE.DAILY;

    private List<DropItem> m_dropItemChestDailyList = new List<DropItem>();
    private List<DropItem> m_dropItemChestCommonList = new List<DropItem>();
    private List<DropItem> m_dropItemChestRareList = new List<DropItem>();
    private List<DropItem> m_dropItemChestEpicList = new List<DropItem>();

    const string k_chestDaily_file = "database/MMB_ChestDaily";
    const string k_chestCommon_file = "database/MMB_ChestCommon";
    const string k_chestRare_file = "database/MMB_ChestRare";
    const string k_chestEpic_file = "database/MMB_ChestEpic";

    public bool IsLoaded { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        LoadAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadAll()
    {

        TextAsset bin = Resources.Load(k_chestDaily_file) as TextAsset;
        TextAsset bin2 = Resources.Load(k_chestCommon_file) as TextAsset;
        TextAsset bin3 = Resources.Load(k_chestRare_file) as TextAsset;
        TextAsset bin4 = Resources.Load(k_chestEpic_file) as TextAsset;

        if (!bin || !bin2 || !bin3 || !bin4)
        {
            Debug.LogError("GachaDB: Can't load data");
            return;
        }

        LoadDropItem(bin, CHEST_TYPE.DAILY);
        LoadDropItem(bin2, CHEST_TYPE.COMMON);
        LoadDropItem(bin3, CHEST_TYPE.RARE);
        LoadDropItem(bin4, CHEST_TYPE.EPIC);

        IsLoaded = true;
    }

    void LoadDropItem(TextAsset pAsset, CHEST_TYPE type)
    {
        using (MemoryStream stream = new MemoryStream(pAsset.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32(); //Number line 

                for (int i = 0; i < number; i++)
                {
                    string groupName = reader.ReadString();
                    string dropItemID = reader.ReadString();
                    int minDrop = reader.ReadInt32();
                    int maxDrop = reader.ReadInt32();
                    float percentage = reader.ReadSingle();
                    float percentageGroup = reader.ReadSingle();

                    DropItem item = new DropItem(groupName, dropItemID, minDrop, maxDrop, percentage, percentageGroup);
                    switch(type)
                    {
                        case CHEST_TYPE.DAILY:
                            m_dropItemChestDailyList.Add(item);
                            break;
                        case CHEST_TYPE.COMMON:
                            m_dropItemChestCommonList.Add(item);
                            break;
                        case CHEST_TYPE.RARE:
                            m_dropItemChestRareList.Add(item);
                            break;
                        case CHEST_TYPE.EPIC:
                            m_dropItemChestEpicList.Add(item);
                            break;
                    }

                }
            }
        }
    }

    public List<DropItem> GetListDropItem(CHEST_TYPE type)
    {
        switch (type)
        {
            case CHEST_TYPE.DAILY:
                return m_dropItemChestDailyList;
                break;
            case CHEST_TYPE.COMMON:
                return m_dropItemChestCommonList;
                break;
            case CHEST_TYPE.RARE:
                return m_dropItemChestRareList;
                break;
            case CHEST_TYPE.EPIC:
                return m_dropItemChestEpicList;
                break;
        }
        return m_dropItemChestDailyList;
    }

    public void ChoiceChest(CHEST_TYPE type)
    {
        m_chestSeleted = type;
    }

    public List<DropItem> GetListDropItemSeleted()
    {
        return GetListDropItem(m_chestSeleted);
    }
}
