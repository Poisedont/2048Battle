using System;

public class FriendFacebook
{
    public string m_name;
    public string m_id;
    public int m_score;
    public FriendFacebook(string name, string id)
    {
        m_name = name;
        m_id = id;
        m_score = 0;
    }

    public void SetScore(int score)
    {
        m_score = score;
    }
}
