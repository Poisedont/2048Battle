using UnityEngine;

public class SkillDelete : Skill
{
    [SerializeField] GameObject m_killEffectPrefab;

    GameObject m_killEffect;
    protected override void OnActivate()
    {
    }
    protected override void OnDeactive()
    {

    }

    protected override void OnTrigger()
    {
        // skill effect
        if (!m_killEffect)
        {
            if (m_killEffectPrefab)
            {
                m_killEffect = Instantiate(m_killEffectPrefab, GetWorldTargetPosition(), Quaternion.identity);
            }
        }

        if (m_killEffect)
        {
            m_killEffect.SetActive(true);
        }

        GridManager.Instance.KillOpponentUnitAt(Target.GridRow, Target.GridColumn);

    }

    protected override void OnUpdate()
    {
        if (IsActivated && Target != null)
        {
            //check unit is opponent or not
            var unit = GridManager.Instance.GetUnitAt(Target.GridRow, Target.GridColumn);
            if (unit != null && unit.UnitType == EUnitType.OPPONENT)
            {
                Trigger();
            }
        }
    }

    public override void SetCount()
    {
        base.SetSkillCount(PlayerManager.Instance.Booster_B2);
    }

    public override void DecreaseSkillCount()
    {
        base.DecreaseSkillCount();
        PlayerManager.Instance.Booster_B2--;
        PlayerManager.Instance.SaveProfile();
    }
}