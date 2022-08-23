using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResultConfig
{
    public string Name { get; private set; }
    public int StartWave { get; private set; }
    public int EndWave { get; private set; }

    public static ResultConfig Create(BinaryReader reader)
    {
        ResultConfig config = new ResultConfig();
        config.Name = reader.ReadString();
        config.StartWave = reader.ReadInt32();
        config.EndWave = reader.ReadInt32();

        return config;
    }

    public string IsInResult(int wave)
    {
        if(wave >= StartWave && wave <= EndWave)
        {
            return Name;
        }

        return null;
    }
}
