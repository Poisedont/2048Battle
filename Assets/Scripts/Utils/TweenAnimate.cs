using UnityEngine;
using UnityEngine.Events;

public enum ETweenEffect
{
    FROM_LEFT,
    ZOOM_OUT,
    ZOOM_IN,
    EXPAND_VERTICAL,
    EXPAND_HORIZONTAL,
    FADE,
}

public class TweenAnimate : MonoBehaviour
{
    [SerializeField] ETweenEffect m_effect;
    [SerializeField] float m_duration = 0.5f;
    [SerializeField] GameObject m_target;
    [SerializeField] bool m_tweenOnDisable;

    [SerializeField] UnityEvent m_ShowCallback;
    [SerializeField] UnityEvent m_HideCallback;

    public bool IsShow { get; private set; }
    private void OnEnable()
    {
        if (m_target == null)
        {
            m_target = gameObject;
        }
        LTDescr desc = ShowEffect(m_effect);
        if (desc != null)
        {
            if (m_ShowCallback != null)
            {
                desc.setOnComplete(() =>
                {
                    m_ShowCallback.Invoke();
                });
            }
        }
    }

    private void OnDisable()
    {
        if (m_tweenOnDisable)
        {
            LTDescr desc = HideEffect();
            if (desc != null)
            {
                if (m_HideCallback != null)
                {
                    desc.setOnComplete(() =>
                    {
                        m_HideCallback.Invoke();
                    });
                }
            }
        }
        else
        {
            if (m_HideCallback != null)
            {
                m_HideCallback.Invoke();
            }
        }
    }

    LTDescr ShowEffect(ETweenEffect p_effect)
    {
        LTDescr ret = null;
        switch (p_effect)
        {
            case ETweenEffect.FROM_LEFT:
                {
                    m_target.transform.position = new Vector3(-Screen.width >> 1, Screen.height >> 1, 0);
                    ret = LeanTween.moveLocalX(m_target, 0, m_duration)
                        .setEase(LeanTweenType.easeOutBack);
                }
                break;
            case ETweenEffect.ZOOM_OUT:
                {
                    m_target.transform.localScale = Vector3.zero;
                    ret = LeanTween.scale(m_target, Vector3.one, m_duration)
                        .setEase(LeanTweenType.easeOutBack);
                }
                break;
            case ETweenEffect.ZOOM_IN:
                {
                    m_target.transform.localScale = Vector3.one * 5;
                    ret = LeanTween.scale(m_target, Vector3.one, m_duration)
                        .setEase(LeanTweenType.easeOutBack);
                }
                break;
            case ETweenEffect.EXPAND_VERTICAL:
                {
                    m_target.transform.localScale = Vector3.right;
                    ret = LeanTween.scale(m_target, Vector3.one, m_duration)
                        .setEase(LeanTweenType.easeOutQuart);
                }
                break;
            case ETweenEffect.EXPAND_HORIZONTAL:
                {
                    m_target.transform.localScale = Vector3.up;
                    ret = LeanTween.scale(m_target, Vector3.one, m_duration)
                        .setEase(LeanTweenType.easeOutQuart);
                }
                break;
            case ETweenEffect.FADE:
                {
                    RectTransform rectTransform = m_target.GetComponent<RectTransform>();
                    if (rectTransform) //is UI
                    {
                        CanvasGroup canvas = rectTransform.GetComponent<CanvasGroup>();
                        if (!canvas)
                        {
                            canvas = m_target.AddComponent<CanvasGroup>();
                        }
                        canvas.alpha = 0;
                        ret = LeanTween.alphaCanvas(canvas, 1, m_duration)
                                .setEase(LeanTweenType.easeInExpo);
                    }
                    else
                    {
                        ret = LeanTween.color(m_target, Color.white, m_duration)
                            .setEase(LeanTweenType.easeInExpo);
                    }
                }
                break;
        }
        return ret;
    }

    LTDescr HideEffect()
    {
        LTDescr ret = null;
        switch (m_effect)
        {
            case ETweenEffect.FROM_LEFT:
                {
                    ret = LeanTween.moveLocalX(m_target, -Screen.width >> 1, m_duration)
                        .setEase(LeanTweenType.easeInBack);
                }
                break;
            case ETweenEffect.ZOOM_OUT:
                {
                    ret = LeanTween.scale(m_target, Vector3.zero, m_duration)
                        .setEase(LeanTweenType.easeInBack);
                }
                break;
            case ETweenEffect.ZOOM_IN:
                {
                    ret = LeanTween.scale(m_target, Vector3.one * 5, m_duration)
                        .setEase(LeanTweenType.easeInBack);
                }
                break;
            case ETweenEffect.EXPAND_VERTICAL:
                {
                    ret = LeanTween.scale(m_target, Vector3.right, m_duration)
                        .setEase(LeanTweenType.easeInQuart);
                }
                break;
            case ETweenEffect.EXPAND_HORIZONTAL:
                {
                    ret = LeanTween.scale(m_target, Vector3.up, m_duration)
                        .setEase(LeanTweenType.easeInQuart);
                }
                break;
            case ETweenEffect.FADE:
                {
                    RectTransform rectTransform = m_target.GetComponent<RectTransform>();
                    if (rectTransform) //is UI
                    {
                        CanvasGroup canvas = rectTransform.GetComponent<CanvasGroup>();
                        if (!canvas)
                        {
                            canvas = m_target.AddComponent<CanvasGroup>();
                        }
                        ret = LeanTween.alphaCanvas(canvas, 0, m_duration)
                                .setEase(LeanTweenType.easeOutExpo);
                    }
                    else
                    {
                        ret = LeanTween.color(m_target, Color.white, m_duration)
                            .setEase(LeanTweenType.easeOutExpo);
                    }
                }
                break;
        }
        return ret;
    }
}
