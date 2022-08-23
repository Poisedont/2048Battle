using System.IO;

public class ShopItemConfig
{
    #region Properties
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string DECS { get; private set; }
    public int Quantity { get; private set; }
    public float Price { get; private set; }
    public string Currency { get; private set; }
    #endregion

    public static ShopItemConfig Create(BinaryReader reader)
    {
        ShopItemConfig config = new ShopItemConfig();
        config.ID = reader.ReadString();
        config.Name = reader.ReadString();
        config.DECS = reader.ReadString();
        config.Quantity = reader.ReadInt32();
        config.Price = reader.ReadSingle();
        config.Currency = reader.ReadString();

        return config;
    }
}