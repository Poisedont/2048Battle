using System.IO;

public class WaveInfo
{
    #region Properties
    public int ID { get; private set; }
    public int Point { get; private set; }
    public int Gold { get; private set; }
    public int PrepareTime { get; private set; }
    public int BattleTurns { get; private set; }
    public int Quantity { get; private set; }

    public float[] QuantityRatioEachTurn { get; private set; }
    public float[] LevelRangeRatio { get; private set; }
    #endregion
    ////////////////////////////////////////////////////////////////////////////////

    public WaveInfo()
    {

    }

    public static WaveInfo Create(BinaryReader reader)
    {
        WaveInfo wave = new WaveInfo();

        wave.ID = reader.ReadInt32();
        wave.Point = reader.ReadInt32();
        wave.Gold = reader.ReadInt32();
        wave.PrepareTime = reader.ReadInt32();
        wave.BattleTurns = reader.ReadInt32();
        wave.Quantity = reader.ReadInt32();

        wave.QuantityRatioEachTurn = new float[GameConst.k_max_opponent_per_turn];
        for (int i = 0; i < wave.QuantityRatioEachTurn.Length; i++)
        {
            wave.QuantityRatioEachTurn[i] = reader.ReadInt32();
        }

        wave.LevelRangeRatio = new float[GameConst.k_max_level_opponent];
        for (int i = 0; i < wave.LevelRangeRatio.Length; i++)
        {
            wave.LevelRangeRatio[i] = reader.ReadInt32();
        }

        return wave;
    }
}