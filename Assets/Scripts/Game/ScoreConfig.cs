using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreConfig", menuName = "2048remake/ScoreConfig")]
public class ScoreConfig : ScriptableObject
{
    public int[] ScorePerLevels;

    const string k_score_file = "database/MMB_EnemyInfo";
    ////////////////////////////////////////////////////////////////////////////////
    private List<OpponentScoreInfo> m_scorePerLevel;
    public bool LoadConfig()
    {
        m_scorePerLevel = new List<OpponentScoreInfo>();

        TextAsset bin = Resources.Load(k_score_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("ScoreConfig: Can't load data " + k_score_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    OpponentScoreInfo info = OpponentScoreInfo.Create(reader);
                    m_scorePerLevel.Add(info);
                }
            }
        }
        return true;
    }

    public int GetPointOfLevel(int level)
    {
        if (level < 0 || level >= m_scorePerLevel.Count)
        {
            return 0;
        }

        return m_scorePerLevel[level].Point;
    }

    public int GetGoldOfLevel(int level)
    {
        if (level < 0 || level >= m_scorePerLevel.Count)
        {
            return 0;
        }

        return m_scorePerLevel[level].Gold;
    }
}

public class OpponentScoreInfo
{
    #region Properties
    public string ID { get; private set; }
    public int Point { get; private set; }
    public int Gold { get; private set; }
    #endregion

    public static OpponentScoreInfo Create(BinaryReader reader)
    {
        OpponentScoreInfo info = new OpponentScoreInfo();

        info.ID = reader.ReadString();
        info.Point = reader.ReadInt32();
        info.Gold = reader.ReadInt32();
        return info;
    }
}