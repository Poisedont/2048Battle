using UnityEngine;

public class SkillLevelUp : Skill
{
    [SerializeField] GameObject m_EffectPrefab;

    GameObject m_effect;
    protected override void OnActivate()
    {
    }
    protected override void OnDeactive()
    {

    }

    protected override void OnTrigger()
    {
        // skill effect
        if (!m_effect)
        {
            if (m_EffectPrefab)
            {
                m_effect = Instantiate(m_EffectPrefab, GetWorldTargetPosition(), Quaternion.identity);
            }
        }

        if (m_effect)
        {
            m_effect.SetActive(true);
            m_effect.transform.position = GetWorldTargetPosition();
        }
    }

    protected override void OnUpdate()
    {
        if (IsActivated && Target != null)
        {
            //check unit is opponent or not
            var unit = GridManager.Instance.GetUnitAt(Target.GridRow, Target.GridColumn);
            int highestLevel = GridManager.Instance.GetHighestAllyLevel();
            if (unit != null && unit.UnitType == EUnitType.ALLY && unit.Level < highestLevel)
            {
                if (GridManager.Instance.LevelUpUnitAt(Target.GridRow, Target.GridColumn))
                {
                    Trigger();
                }
            }
        }
    }

    public override void SetCount()
    {
        base.SetSkillCount(PlayerManager.Instance.Booster_B1);
    }

    public override void DecreaseSkillCount()
    {
        base.DecreaseSkillCount();
        PlayerManager.Instance.Booster_B1--;
        PlayerManager.Instance.SaveProfile();
    }
}
