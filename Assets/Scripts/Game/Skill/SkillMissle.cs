using System;
using UnityEngine;

public class SkillMissle : Skill
{
    [Header("Missle config")]
    [SerializeField] Missle m_missle;

    Missle m_missleObject;
    protected override void OnActivate()
    {
    }
    protected override void OnDeactive()
    {

    }

    protected override void OnTrigger()
    {
        if (!m_missleObject)
        {
            m_missleObject = Instantiate<Missle>(m_missle, GetWorldTargetPosition(), Quaternion.identity);
        }
        if (m_missleObject)
        {
            m_missleObject.gameObject.SetActive(true);
            m_missleObject.transform.position = GetWorldTargetPosition();
            m_missleObject.Fire();
        }

    }

    protected override void OnUpdate()
    {
        if (IsActivated && Target != null)
        {
            Trigger();
        }

        if (m_missleObject && m_missleObject.gameObject.activeSelf && m_missleObject.OnTargetReached)
        {
            m_missleObject.gameObject.SetActive(false);
            Triggered = false;

            // kill some enemies
            DamageOnUnits();

        }
    }

    private void DamageOnUnits()
    {
        GridManager.Instance.KillOpponentUnitAt(Target.GridRow, Target.GridColumn);
    }
}