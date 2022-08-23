using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchControll : MonoBehaviour
{
    private bool m_dragging;
    private Vector3 m_startDragPos;

    private float m_CosAngle;

    [Serializable]
    class TouchDragEvent : UnityEvent<EDirection> { }
    [Header("Drag event")]
    [SerializeField] TouchDragEvent m_DragEvent;

    [SerializeField] bool m_keySupport;

    void Start()
    {
        m_CosAngle = Mathf.Cos(Mathf.Deg2Rad * 45); //45 degree
    }

    void Update()
    {
        UpdateTouchDrag();
        if (m_keySupport)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_DragEvent.Invoke(EDirection.Up);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_DragEvent.Invoke(EDirection.Down);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                m_DragEvent.Invoke(EDirection.Left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                m_DragEvent.Invoke(EDirection.Right);
            }

        }
    }
    ////////////////////////////////////////////////////////////////////////////////
    private void OnMouseDown()
    {
        m_dragging = true;
        m_startDragPos = Input.mousePosition;
    }
    private void OnMouseUp()
    {
        m_dragging = false;
    }
    private void UpdateTouchDrag()
    {
        if (!m_dragging) { return; }

        if (Input.GetMouseButton(0))
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            float lengDrag = (Input.mousePosition - m_startDragPos).sqrMagnitude / 1000;

            if (lengDrag < 1) return;

            Vector3 diff = (Input.mousePosition - m_startDragPos).normalized;

            float dotProduct = Vector3.Dot(diff, Vector3.right);
            if (diff != Vector3.zero)
            {
                if (Mathf.Abs(dotProduct) > m_CosAngle) //horizontal
                {
                    if (diff.x < 0)
                    {
                        if (m_DragEvent != null)
                        {
                            m_DragEvent.Invoke(EDirection.Left);
                            m_dragging = false;
                        }
                    }
                    else
                    {
                        if (m_DragEvent != null)
                        {
                            m_DragEvent.Invoke(EDirection.Right);
                            m_dragging = false;
                        }
                    }
                }
                else //vertical move
                {
                    if (diff.y < 0)
                    {
                        if (m_DragEvent != null)
                        {
                            m_DragEvent.Invoke(EDirection.Down);
                            m_dragging = false;
                        }
                    }
                    else
                    {
                        if (m_DragEvent != null)
                        {
                            m_DragEvent.Invoke(EDirection.Up);
                            m_dragging = false;
                        }
                    }
                }
            }
        }
    }

    
}
