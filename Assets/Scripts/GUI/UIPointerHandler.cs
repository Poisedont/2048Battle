using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] UnityEvent PointerDownEvent;
    [SerializeField] UnityEvent PointerUpEvent;
    [SerializeField] UnityEvent PointerEnterEvent;
    [SerializeField] UnityEvent PointerExitEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PointerDownEvent != null)
        {
            PointerDownEvent.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PointerEnterEvent != null)
        {
            PointerEnterEvent.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (PointerExitEvent != null)
        {
            PointerExitEvent.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (PointerUpEvent != null)
        {
            PointerUpEvent.Invoke();
        }
    }
}