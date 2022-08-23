
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "2048remake/SkillData", order = 0)]
public class SkillData : ScriptableObject
{
    public float doubleEvolRatio = 0f;
    public float allyHigherRatio = 0f;
    public float bonusGold = 0f;
    public float bonusGoldPerKill = 0f;
    public float timePrepareAdd = 0f;
    public float turnBattleAdd = 0f;
    public float surviveRatioWhenAtkSameLevelUnit = 0f;
    public float blockEnemyPortalRatio = 0f;

    public void ClearData()
    {
        doubleEvolRatio = 0f;
        allyHigherRatio = 0f; //Allies appear are coming from 1 wave higher. 
        bonusGold = 0f;
        bonusGoldPerKill = 0f;
        timePrepareAdd = 0f;
        turnBattleAdd = 0f;
        surviveRatioWhenAtkSameLevelUnit = 0f;
        blockEnemyPortalRatio = 0f;
    }

    ////////////////////////////////////////////////////////////////////////////////
    public bool ShouldDoubleEvolve()
    {
        float rand = Random.Range(0f, 100f);
        return (rand <= this.doubleEvolRatio);
    }

    public bool ShouldAllyHigherAppear()
    {
        float rand = Random.Range(0f, 100f);
        return (rand <= this.allyHigherRatio);
    }

    public bool ShouldSurviveWhenAttackSameLevelUnit()
    {
        float rand = Random.Range(0f, 100f);
        return rand <= this.surviveRatioWhenAtkSameLevelUnit;
    }

    public bool ShouldBlockEnemyPortal()
    {
        float rand = Random.Range(0f, 100f);
        return rand <= this.blockEnemyPortalRatio;
    }
}