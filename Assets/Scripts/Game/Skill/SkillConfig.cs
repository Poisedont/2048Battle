using UnityEngine;

[CreateAssetMenu(fileName = "SkillConfig", menuName = "2048remake/SkillConfig")]
public class SkillConfig : ScriptableObject
{
    public string BoosterId;
    public ESkillType type = ESkillType.ACTIVE;
    public ESkilRange rangeType = ESkilRange.HORIZONTAL;
    public int range = 1;
    public float coolDown;
    public Sprite icon;

}

public enum ESkillType
{
    ACTIVE,
    PASSIVE,
}

public enum ESkilRange
{
    HORIZONTAL,
    VERTICAL,
    SQUARE
}

public class SkillTarget
{
    public int GridRow { get; set; }
    public int GridColumn { get; set; }
}