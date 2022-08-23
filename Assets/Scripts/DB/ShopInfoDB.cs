using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopInfoDB : Singleton<ShopInfoDB>
{
    private ShopInfoDB() { }

    const string k_shop_info_file = "database/MMB_ShopInfo";

    private List<ShopItemConfig> m_shopConfigs;

    private void Start()
    {
        m_shopConfigs = new List<ShopItemConfig>();

        LoadShopConfig();
    }

    private bool LoadShopConfig()
    {
        TextAsset bin = Resources.Load(k_shop_info_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("ChapterDB: Can't load data " + k_shop_info_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    ShopItemConfig shopItem = ShopItemConfig.Create(reader);
                    m_shopConfigs.Add(shopItem);
                }
            }
        }
        return true;
    }

    public ShopItemConfig GetShopItemConfig(string pID)
    {
        return m_shopConfigs.Find(a => a.ID.Equals(pID));
    }
}