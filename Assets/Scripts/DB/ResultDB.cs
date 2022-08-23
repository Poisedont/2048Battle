using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResultDB : Singleton<ResultDB>
{
    private ResultDB() { }

    const string k_result_info_file = "database/MMB_ResultScr";

    private List<ResultConfig> m_resultConfigs;

    private void Start()
    {
        m_resultConfigs = new List<ResultConfig>();

        LoadResultConfig();
    }

    private bool LoadResultConfig()
    {
        TextAsset bin = Resources.Load(k_result_info_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("ChapterDB: Can't load data " + k_result_info_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    ResultConfig result = ResultConfig.Create(reader);
                    m_resultConfigs.Add(result);
                }
            }
        }
        return true;
    }

    public string GetResultStrConfig(int pWave)
    {
        string resultStr = null;
        for (int i = 0; i< m_resultConfigs.Count; i++)
        {
            resultStr = m_resultConfigs[i].IsInResult(pWave);
            if (resultStr != null)
            {
                return resultStr;
            }
        }
        return null;
    }
}
