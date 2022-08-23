using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapItem : MonoBehaviour
{

    [SerializeField] MainMenu m_mainMenu;
    [SerializeField] Image m_bgMap;
    [SerializeField] Text m_name;
    [SerializeField] Text m_unlockText;
    [SerializeField] Image m_PickIcon;
    [SerializeField] Image m_dropIcon;
    [SerializeField] int m_indexMap;
    [SerializeField] int m_numberPassWaeToUnlock;

    [SerializeField] List<Sprite> m_listMap;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_indexMap == 0
        || m_indexMap == 1 && PlayerManager.Instance.BestWavePass >= ChapterDB.Instance.GetChapterConfig(0).NumWavesUnlockNextChap
        || m_indexMap == 2 && PlayerManager.Instance.BestWavePass1 >= ChapterDB.Instance.GetChapterConfig(1).NumWavesUnlockNextChap
        )
        {
            m_bgMap.sprite = m_listMap[1];
            m_name.gameObject.SetActive(true);
            m_unlockText.gameObject.SetActive(false);
            m_dropIcon.gameObject.SetActive(false);
        }
        else
        {
            m_bgMap.sprite = m_listMap[0];
            m_name.gameObject.SetActive(false);
            m_unlockText.gameObject.SetActive(true);
            m_dropIcon.gameObject.SetActive(true);
        }

        if (PlayerManager.Instance.MapSeleted == m_indexMap)
        {
            m_PickIcon.gameObject.SetActive(true);
        }
        else
        {
            m_PickIcon.gameObject.SetActive(false);
        }
    }

    public void OnMapClick()
    {
        if (!m_dropIcon.IsActive())
        {
            PlayerManager.Instance.MapSeleted = m_indexMap;
            GridManager.Instance.UpdateCurrentMap();
            PlayerManager.Instance.SaveProfile();
            m_mainMenu.SetMapName(m_name.text);
            m_mainMenu.SetMapImg(m_listMap[1]);
            m_mainMenu.HideListMap();
        }
    }
}
