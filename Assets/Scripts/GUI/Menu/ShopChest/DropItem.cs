using System;
using rds;

public class DropItem : RDSObject
{
    public string m_groupName;
    public string m_name;
    public int m_minDrop;
    public int m_maxDrop;
    public float m_percentage;
    public float m_percentageGroup;

    public DropItem(string goupName, string name, int minDrop, int maxDrop, float percentage, float percentageGroup)
    {
        m_groupName = goupName;
        m_name = name;
        m_minDrop = minDrop;
        m_maxDrop = maxDrop;
        m_percentage = percentage;
        m_percentageGroup = percentageGroup;
    }

    public override string ToString()
    {
        return m_name;
    }

}
