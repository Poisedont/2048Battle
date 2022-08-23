using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUnitType
{
    ALLY,
    OPPONENT,
    GIFT,
}

public class GridUnit
{
    public EUnitType UnitType { get; set; }
    public int Level { get; set; }
    public bool WasJustDuplicated { get; internal set; }
    public GameObject CurrentGO { get; set; }
    public float RemoveTime { get; set; } // time to remove this, set base on Time.time
    public bool WasDoubleEvolved { get; set; } //use to make animation when double evolve
    public bool WasJustSurvived { get; set; } //use to make animation when survive when merge equal opponent
    ////////////////////////////////////////////////////////////////////////////////
}
