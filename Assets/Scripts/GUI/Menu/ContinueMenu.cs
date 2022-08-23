using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueMenu : MenuBase<ContinueMenu>
{
    [SerializeField] Image m_timeCountImage;

    private const float k_maxTime = 5;
    TimerCount m_time = new TimerCount();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_time.Update();
        float CurrenceTimeCount = m_time.ElapsedTime;

        m_timeCountImage.GetComponent<Image>().fillAmount = CurrenceTimeCount / k_maxTime;

        if(CurrenceTimeCount >= k_maxTime)
        {
            EndTime();
        }
    }

    public void EndTime()
    {
        m_time.Pause();
        m_time.Reset();
        ResultMenu.Open();
    }

    protected override void OnMenuOpening()
    {
        base.OnMenuOpening();
        m_time.Reset();
        m_time.Play();
    }

}
