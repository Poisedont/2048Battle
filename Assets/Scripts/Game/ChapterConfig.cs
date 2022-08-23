using System.IO;

public class ChapterConfig
{
    #region Properties
    public int ID { get; private set; }
    public int NumWavesUnlockNextChap { get; private set; }
    public int OpponentStartWave { get; private set; }
    public int AllyStartWave { get; private set; }
    #endregion

    public static ChapterConfig Create(BinaryReader reader)
    {
        ChapterConfig config = new ChapterConfig();
        config.ID = reader.ReadInt32();
        config.NumWavesUnlockNextChap = reader.ReadInt32();
        config.OpponentStartWave = reader.ReadInt32();
        config.AllyStartWave = reader.ReadInt32();

        return config;
    }
}