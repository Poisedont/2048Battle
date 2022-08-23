using UnityEngine;

public class UnitNormal : MonoBehaviour
{
    [SerializeField] GameObject m_info;

    private void Start() {
        if (m_info)
        {
            m_info.SetActive(false);
        }
    }
    public void ShowInfo(bool show)
    {
        if (m_info)
        {
            m_info.SetActive(show);
        }
    }
}