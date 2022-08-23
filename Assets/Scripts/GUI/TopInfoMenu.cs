using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopInfoMenu : MonoBehaviour
{

    [SerializeField] Text m_HardCurrency;
    [SerializeField] Text m_SoftCurrency;
    // Start is called before the first frame update
    void Start()
    {
        if (m_HardCurrency) m_HardCurrency.text = PlayerManager.Instance.Hard_Currency.ToString();
        if (m_SoftCurrency) m_SoftCurrency.text = PlayerManager.Instance.Soft_Currency.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        if (m_HardCurrency) m_HardCurrency.text = PlayerManager.Instance.Hard_Currency.ToString();
        if (m_SoftCurrency) m_SoftCurrency.text = PlayerManager.Instance.Soft_Currency.ToString();
    }
}
