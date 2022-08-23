using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropItemUI : MonoBehaviour
{

    [SerializeField] Image m_ItemIcon;
    [SerializeField] Text m_numberItem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateIcon(Sprite sprite)
    {
        m_ItemIcon.sprite = sprite;
    }

    public void UpdateNumber(int number)
    {
        m_numberItem.text = number.ToString();
    }
}
