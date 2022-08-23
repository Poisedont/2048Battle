using System.IO;

public class PassiveSkillInfo
{
    #region Properties
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Desc { get; private set; }

    public float[] SkillValues { get; private set; }
    public float[] SkillUpgradePrices { get; private set; }

    public int MaxLevel { get; private set; }

    #endregion  

    public static PassiveSkillInfo Create(BinaryReader reader)
    {
        PassiveSkillInfo info = new PassiveSkillInfo();

        info.ID = reader.ReadString();
        info.Name = reader.ReadString();
        info.Desc = reader.ReadString();

        info.SkillValues = new float[GameConst.k_max_level_skill_passive];
        info.SkillUpgradePrices = new float[GameConst.k_max_level_skill_passive];
        int maxLv = -1;
        for (int i = 0; i < GameConst.k_max_level_skill_passive; i++)
        {
            float min = reader.ReadSingle();
            float max = reader.ReadSingle();
            if (min == 0f && max == 0f && maxLv == -1)
            {
                maxLv = i - 1;
            }
            else
            {
                info.SkillValues[i] = min;
                info.SkillUpgradePrices[i] = max;
            }
        }
        if (maxLv >= 0)
        {
            info.MaxLevel = maxLv;
        }
        else
        {
            info.MaxLevel = info.SkillValues.Length - 1;
        }

        return info;
    }
}