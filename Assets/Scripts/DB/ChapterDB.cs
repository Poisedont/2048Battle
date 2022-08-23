using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChapterDB : Singleton<ChapterDB>
{
    private ChapterDB() { }

    const string k_chapter_info_file = "database/MMB_Chapter";

    private List<ChapterConfig> m_chapterConfigs;

    private void Start()
    {
        m_chapterConfigs = new List<ChapterConfig>();

        LoadChapterConfig();
    }

    private bool LoadChapterConfig()
    {
        TextAsset bin = Resources.Load(k_chapter_info_file) as TextAsset;

        if (!bin)
        {
            Debug.LogError("ChapterDB: Can't load data " + k_chapter_info_file);
            return false;
        }

        using (MemoryStream stream = new MemoryStream(bin.bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int number = reader.ReadInt32();
                for (int i = 0; i < number; i++)
                {
                    ChapterConfig chapter = ChapterConfig.Create(reader);
                    m_chapterConfigs.Add(chapter);
                }
            }
        }
        return true;
    }

    public ChapterConfig GetChapterConfig(int index)
    {
        return m_chapterConfigs.Find(a => a.ID == index);
    }
}