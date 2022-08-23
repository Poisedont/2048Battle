using UnityEngine;

public class SkillUndo : Skill
{
    protected override void OnActivate()
    {
    }
    protected override void OnDeactive()
    {

    }

    protected override void OnTrigger()
    {
        EffectManager.GetNextObjectWithName("undo", true);
        GridManager.Instance.UndoLastMove();
    }

    protected override void OnUpdate()
    {
        if (IsActivated && Target != null)
        {
            if (GridManager.Instance.CanUndoLastMove())
            {
                Trigger();
            }
        }
    }

    public override void SetCount()
    {
        base.SetSkillCount(PlayerManager.Instance.Booster_B3);
    }

    public override void DecreaseSkillCount()
    {
        base.DecreaseSkillCount();
        PlayerManager.Instance.Booster_B3--;
        PlayerManager.Instance.SaveProfile();
    }
}