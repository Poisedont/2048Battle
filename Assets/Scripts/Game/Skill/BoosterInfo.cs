using System.IO;

public class BoosterInfo
{
    #region Properties
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Desc { get; private set; }
    #endregion

    public static BoosterInfo Create(BinaryReader reader)
    {
        BoosterInfo info = new BoosterInfo();

        info.ID = reader.ReadString();
        info.Name = reader.ReadString();
        info.Desc = reader.ReadString();
        return info;
    }
}