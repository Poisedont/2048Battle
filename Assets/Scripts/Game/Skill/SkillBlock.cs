using UnityEngine;

public class SkillBlock : Skill
{
    protected override void OnActivate()
    {
    }
    protected override void OnDeactive()
    {

    }

    protected override void OnTrigger()
    {

    }

    protected override void OnUpdate()
    {
        if (IsActivated && Target != null)
        {
            if (GridManager.Instance.BlockGridPortal(Target.GridRow, Target.GridColumn))
            {
                Trigger();
            }
        }
    }

    public override void SetCount()
    {
        base.SetSkillCount(PlayerManager.Instance.Booster_B4);
    }

    public override void DecreaseSkillCount()
    {
        base.DecreaseSkillCount();
        PlayerManager.Instance.Booster_B4--;
        PlayerManager.Instance.SaveProfile();
    }
}