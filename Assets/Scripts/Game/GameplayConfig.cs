using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayConfig", menuName = "2048remake/GameplayConfig", order = 0)]
public class GameplayConfig : ScriptableObject
{
    [SerializeField] TimeOpponentGenerate[] timeOpponentGenerate;
    [Tooltip("Rate spawn gift each wave")]
    [Range(0, 100)]
    [SerializeField] float m_rateSpawnGift = 0;

    ////////////////////////////////////////////////////////////////////////////////
    const string k_wave_info_file = "database/MMB_WaveInfo";
    const string k_ally_info_file = "database/MMB_AllySpawn";
    ////////////////////////////////////////////////////////////////////////////////
    private List<WaveInfo> m_waveInfo;
    private List<AllySpawnInfo> m_allySpawnInfo;
    ////////////////////////////////////////////////////////////////////////////////

    #region Load asset
    public void LoadConfig()
    {
        m_waveInfo = new List<WaveInfo>();
        m_allySpawnInfo = new List<AllySpawnInfo>();

        LoadWaveInfo();
        LoadAllyInfo();
    }

    private bool LoadAllyInfo()
    {
        TextAsset bin = Resources.Load(k_ally_info_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("GameplayConfig: Can't load data " + k_ally_info_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    AllySpawnInfo info = AllySpawnInfo.Create(reader);

                    m_allySpawnInfo.Add(info);
                }
            }
        }
        return true;
    }

    private bool LoadWaveInfo()
    {
        TextAsset bin = Resources.Load(k_wave_info_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("GameplayConfig: Can't load data " + k_wave_info_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    WaveInfo wave = WaveInfo.Create(reader);

                    m_waveInfo.Add(wave);
                }
            }
        }
        return true;
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////
    #region Get info methods
    public float GetNextTimeForNewWave(int wave)
    {
        if (wave >= m_waveInfo.Count)
        {
            return (m_waveInfo[m_waveInfo.Count - 1].PrepareTime);
        }
        if (wave < 0) return 0;
        return m_waveInfo[wave].PrepareTime;
    }

    [System.Obsolete("This is useless method that copy from old source code")]
    public float GetTimeSpendedForWave(int wave)
    {
        if (wave >= timeOpponentGenerate.Length)
        {
            wave = timeOpponentGenerate.Length - 1;
        }
        if (wave < 0) return 0;

        float total = 0;
        if (wave - 1 >= 0)
        {
            for (int i = 0; i < wave; i++)
            {
                total += timeOpponentGenerate[i].times;
            }
        }

        return timeOpponentGenerate[wave].times - total;
    }

    public int GetNumOpponentNextWave(int wave)
    {
        if (wave < 0) return 0;
        if (wave >= m_waveInfo.Count)
        {
            return m_waveInfo[m_waveInfo.Count - 1].Quantity;
        }

        return m_waveInfo[wave].Quantity;
    }

    public int GetNumOpponentInTurn(int wave)
    {
        if (wave >= m_waveInfo.Count)
        {
            wave = m_waveInfo.Count - 1;
        }
        if (wave < 0) return 1;
        int rand = Random.Range(0, 100);
        float ratio = 0;
        for (int i = 0; i < m_waveInfo[wave].QuantityRatioEachTurn.Length; i++)
        {
            if (rand <= ratio + m_waveInfo[wave].QuantityRatioEachTurn[i] && rand > ratio)
            {
                return i + 1;
            }
            ratio += m_waveInfo[wave].QuantityRatioEachTurn[i];
        }
        return 1;
    }

    public int GetLevelOppnentInTurn(int wave)
    {
        if (wave < 0) return 1;
        if (wave >= m_waveInfo.Count)
        {
            wave = m_waveInfo.Count - 1;
        }

        int rand = Random.Range(0, 100);
        float ratio = 0;
        for (int i = 0; i < m_waveInfo[wave].LevelRangeRatio.Length; i++)
        {
            if (rand <= ratio + m_waveInfo[wave].LevelRangeRatio[i] && rand > ratio)
            {
                // Debug.Log("LV OPPONENTS GENERATE: " + (i + 1) + " :Wave: " + wave + ": Rand: " + rand);

                return i;
            }
            ratio += m_waveInfo[wave].LevelRangeRatio[i];
        }
        return 0;
    }

    public int GetLevelSpawnAlly(int wave)
    {
        if (wave < 0) return 1;
        if (wave >= m_allySpawnInfo.Count)
        {
            wave = m_allySpawnInfo.Count - 1;
        }

        int rand = Random.Range(0, 100);
        float ratio = 0;
        for (int i = 0; i < m_allySpawnInfo[wave].LevelSpawnRates.Length; i++)
        {
            if (rand <= ratio + m_allySpawnInfo[wave].LevelSpawnRates[i] && rand > ratio)
            {
                // Debug.Log("LV ally GENERATE: " + (i + 1) + " :Wave: " + wave + ": Rand vl: " + rand);

                return i;
            }
            ratio += m_allySpawnInfo[wave].LevelSpawnRates[i];
        }
        return 0;
    }

    public float GetRateSpawnGift()
    {
        return m_rateSpawnGift;
    }

    public int GetMaxTurn(int wave)
    {
        if (wave < 0) return 1;
        if (wave >= m_waveInfo.Count)
        {
            wave = m_waveInfo.Count - 1;
        }

        return m_waveInfo[wave].BattleTurns;
    }
    #endregion
}

[System.Serializable]
public class TimeOpponentGenerate
{
    [Tooltip("Prepare time before opponent spawn")]
    public float times;
    public int quantity;
    [Range(0, 100)]
    public float[] quantityRatioEachTurn;
    [Range(0, 100)]
    public float[] levelRangeRatio;
}

public class AllySpawnInfo
{
    #region Properties
    public int WaveID { get; private set; }
    public float[] LevelSpawnRates { get; private set; }
    #endregion

    public static AllySpawnInfo Create(BinaryReader reader)
    {
        AllySpawnInfo info = new AllySpawnInfo();

        info.WaveID = reader.ReadInt32();
        info.LevelSpawnRates = new float[GameConst.k_max_level_ally_spawn];
        for (int i = 0; i < info.LevelSpawnRates.Length; i++)
        {
            info.LevelSpawnRates[i] = reader.ReadInt32();
        }
        return info;
    }
}