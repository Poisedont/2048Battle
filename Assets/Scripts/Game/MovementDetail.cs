using UnityEngine;

public class MovementDetail
{
    public GameObject GOToAnimateScale { get; private set; }
    public GameObject GOToAnimatePosition { get; private set; }
    public GameObject GOToAnimateAttack { get; private set; }
    public int NewRow { get; private set; }
    public int NewColumn { get; private set; }
    /// <summary>
    /// Determine that movement should kill all units. Usually it will be false.
    /// It will be true in case two unit is attack and kill together, then no new unit should spawn
    /// </summary>
    public bool ShouldKillAllUnits { get; private set; }

    public MovementDetail(int newRow, int newColumn, 
        GameObject goToAnimatePosition, GameObject goToAnimateScale, GameObject goToAnimAttack, bool killAll = false)
    {
        NewRow = newRow;
        NewColumn = newColumn;
        GOToAnimatePosition = goToAnimatePosition;
        GOToAnimateScale = goToAnimateScale;
        GOToAnimateAttack = goToAnimAttack;
        ShouldKillAllUnits = killAll;
    }
}