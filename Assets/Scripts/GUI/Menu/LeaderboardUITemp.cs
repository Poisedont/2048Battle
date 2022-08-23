using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUITemp : MonoBehaviour
{
    [SerializeField] List<Sprite> m_listRankIcon;

    [SerializeField] Image m_rankIcon;
    [SerializeField] Text m_rankText;
    [SerializeField] Image m_avatar;
    [SerializeField] Text m_name;
    [SerializeField] Text m_score;

    public LeaderboardUITemp(Image pAvatar, string name, string score)
    {
        m_avatar = pAvatar;
        m_name.text = name;
        m_score.text = score;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAvatar(Sprite avatar)
    {
        m_avatar.sprite = avatar;
    }

    public void SetName(string name)
    {
        m_name.text = name;
    }

    public void SetScore(string score)
    {
        m_score.text = score;
    }

    public void SetRank(int rank)
    {
        m_rankText.text = (rank+1).ToString();

        int index = rank;
        if(index >= 3)
        {
            index = 3;
        }

        m_rankIcon.sprite = m_listRankIcon[index];

    }
}
