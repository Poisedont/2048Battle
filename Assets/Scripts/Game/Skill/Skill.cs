using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField] protected SkillConfig m_config;
    [SerializeField] protected Toggle m_toggle;
    #endregion

    #region Privates fields
    protected float m_currentCoolDown;
    protected BoosterInfo m_boosterInfo;
    #endregion

    #region Properties
    public float CurrentCoolDown { get { return m_currentCoolDown; } }
    public float CoolDownPercent
    {
        get
        {
            return m_currentCoolDown / m_config.coolDown;
        }
    }
    public SkillTarget Target { get; set; }
    public bool IsActivated { get; protected set; }
    public bool Triggered { get; protected set; }
    public int AvailableCount { get; private set; }

    public Sprite SkillIcon { get { return m_config.icon; } }
    public string BoosterId { get { return m_config.BoosterId; } }
    public BoosterInfo BoosterInfo { get { return m_boosterInfo; } }

    public UnityAction SkillValueChanged;
    #endregion
    ////////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        //assign booster info
        if (m_config)
        {
            m_boosterInfo = SkillController.Instance.GetBoosterInfo(m_config.BoosterId);
            if (m_boosterInfo == null)
            {
                Debug.LogError("Error: can't assign booster info : " + m_config.BoosterId);
            }
        }
    }
    public void OnToggleSkill(bool isOn)
    {
        if (isOn)
        {
            if (m_currentCoolDown == 0 && AvailableCount > 0)
                Activate();
            else
            {
                m_toggle.isOn = false;
            }
        }
        else
        {
            DeActivate();
        }
    }
    /// <summary>
    /// Make the skill active. It mean turn it on but still not cast (trigger)
    /// </summary>
    public void Activate()
    {
        // skill can be actived only it already cooldown
        if (m_currentCoolDown == 0 && AvailableCount > 0)
        {
            OnActivate();
            IsActivated = true;

            SkillController.Instance.ActiveSkill = this;
            Target = null;

            Debug.Log("active skill " + m_boosterInfo.Name);

            if (m_toggle)
            {
                m_toggle.isOn = true;
            }
        }
    }

    /// <summary>
    /// Make the skill in-active. It mean turn it off.
    /// </summary>
    public void DeActivate()
    {
        if (IsActivated)
        {
            OnDeactive();
            IsActivated = false;

            Debug.Log("de-active skill " + m_boosterInfo.Name);
            SkillController.Instance.ActiveSkill = null;

            if (m_toggle)
            {
                m_toggle.isOn = false;
            }
        }
    }

    /// <summary>
    /// Trigger an skill that was turned on
    /// </summary>
    public void Trigger()
    {
        if (IsActivated)
        {
            m_currentCoolDown = m_config.coolDown;
            Debug.Log("trigger skill " + m_boosterInfo.Name);
            Triggered = true;
            DecreaseSkillCount();

            OnTrigger();

            DeActivate();

            SkillController.Instance.ActiveSkill = null;
        }
    }

    public void Update()
    {
        if (m_currentCoolDown > 0)
        {
            m_currentCoolDown -= Time.deltaTime;

            if (m_currentCoolDown < 0) m_currentCoolDown = 0;
        }

        OnUpdate();
    }

    public Vector3 GetWorldTargetPosition()
    {
        if (Target != null)
        {
            return GridManager.Instance.ConvertGridToWorldPos(Target.GridRow, Target.GridColumn);
        }
        return Vector3.zero;
    }

    public void IncreaseSkillCount()
    {
        AvailableCount++;
        //update skill count ui
        if (SkillValueChanged != null)
        {
            SkillValueChanged();
        }
    }
    public void SetSkillCount(int pCount)
    {
        AvailableCount = pCount;
        if (SkillValueChanged != null)
        {
            SkillValueChanged();
        }
    }

    public virtual void SetCount()
    { 
    }

    public virtual void DecreaseSkillCount()
    {
        AvailableCount--;
        if (SkillValueChanged != null)
        {
            SkillValueChanged();
        }

    }

    public void Reset()
    {
        m_currentCoolDown = 0;
        m_toggle.isOn = false;
    }
    ////////////////////////////////////////////////////////////////////////////////
    #region Virtual methods
    protected virtual void OnActivate()
    { }

    protected virtual void OnTrigger()
    { }

    protected virtual void OnUpdate()
    { }

    protected virtual void OnDeactive()
    { }
    #endregion
}
