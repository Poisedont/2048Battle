using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Flags]
public enum EGridFlag
{
    NONE = 0,
    Block = 1 << 0,
}

public class GridManager : Singleton<GridManager>
{
    [Header("Please always set Grid at position (0,0,0).")]
    [Space]
    [Tooltip("Number of row")]
    [SerializeField] int m_row;
    [Tooltip("Number of column")]
    [SerializeField] int m_column;
    [SerializeField] float m_blockSize = 1;
    [SerializeField] float m_spacing;

    [SerializeField] GameObject m_testObject;
    [SerializeField] GameObject m_preSpawnEffect;
    [Tooltip("Time need for each movement of an unit")]
    [SerializeField] float m_animTimeUnit = GameConst.k_gridUnit_move_item;

    [Space]
    [Header("Unit prefabs")]
    [SerializeField] List<GameObject> m_allyUnits;
    [SerializeField] List<GameObject> m_oponentUnits;
    [SerializeField] GameObject m_giftUnit;

    [SerializeField] ScoreConfig m_scoreConfig;

    [Header("Grid debug config")]
    [SerializeField] Text m_debugText;
    [SerializeField] bool m_showConsole;
    [SerializeField] private bool m_attackReduceLevel;


    [Header("MAP")]
    [SerializeField] List<Sprite> m_maps;
    [SerializeField] SpriteRenderer m_currentMap;

    [Header("Player Passive skill data")]
    [SerializeField] SkillData m_skillData;
    ////////////////////////////////////////////////////////////////////////////////

    GridUnit[] m_gridUnits;
    EGridFlag[] m_gridFlags;
    GridCell[] m_gridCells;

    GridState m_gridState;
    bool m_hasSavedState;

    float m_gridWidth, m_gridHeight;
    List<int> m_considerOpponentPos;
    List<GameObject> m_preSpawnEffectList;
    List<GameObject> m_blockEffectList;

    #region Properties
    public bool IsMovingGrid { get; private set; }

    public int RowNumber { get { return m_row; } }
    public int ColumnNumber { get { return m_column; } }
    public int GridLength { get { return m_row * m_column; } }

    public bool OpenedGift { get; private set; }
    #endregion

    void Start()
    {
        m_scoreConfig.LoadConfig();

        m_gridUnits = new GridUnit[GridLength];

        m_gridFlags = new EGridFlag[GridLength];
        m_gridCells = new GridCell[GridLength];

        m_gridState = new GridState(GridLength);

        m_gridWidth = m_blockSize * m_column + (m_column - 1) * m_spacing;
        m_gridHeight = m_blockSize * m_row + (m_row - 1) * m_spacing;

        GenerateGrid();

        m_considerOpponentPos = new List<int>();
        m_preSpawnEffectList = new List<GameObject>();
        {
            if (m_preSpawnEffect)
            {
                for (int i = 0; i < 5; i++) //TODO: pre define pool size
                {
                    GameObject obj = Instantiate(m_preSpawnEffect, Vector3.zero, Quaternion.identity);
                    obj.SetActive(false);
                    m_preSpawnEffectList.Add(obj);
                }
            }
        }

        string str = ShowMatrixOnConsole();
        ShowDebug(str);

        QualitySettings.vSyncCount = 1;
    }

    private void Update()
    {
        // CheckGiftTime();
    }

    public void UpdateCurrentMap()
    {
        m_currentMap.sprite = m_maps[PlayerManager.Instance.MapSeleted];
    }

    private void CheckGiftTime()
    {
        for (int i = 0; i < m_gridUnits.Length; i++)
        {
            if (m_gridUnits[i] != null)
            {
                if (m_gridUnits[i].UnitType == EUnitType.GIFT)
                {
                    if (m_gridUnits[i].RemoveTime <= Time.time || OpenedGift) // gift over time or opened
                    {
                        var unitGo = m_gridUnits[i].CurrentGO;
                        if (unitGo)
                        {
                            Destroy(unitGo);
                        }
                        m_gridUnits[i] = null;
                    }
                }
            }
        }
    }

    public bool BlockGridPortal(int row, int column)
    {
        if (HasFlag(EGridFlag.Block, row, column))
        {
            return false; //can't block cell that was blocked
        }
        int idx = row * m_column + column;

        var portals = GameplayController.Instance.CurrentPortals;
        if (portals.Contains(idx))
        {
            SetFlag(EGridFlag.Block, row, column);

            GameObject vfx = EffectManager.GetNextObjectWithName("gridBlock", true);
            if (vfx)
            {
                m_gridCells[idx].BlockEffect = vfx;

                vfx.transform.position = m_gridCells[idx].transform.position;
            }

            return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////
    #region Methods Unit spawn
    void GenerateGrid()
    {
        for (int row = 0; row < m_row; row++)
        {
            for (int column = 0; column < m_column; column++)
            {
                var obj = Instantiate(m_testObject, ConvertGridToWorldPos(row, column), Quaternion.identity);
                obj.SetActive(true);

                GridCell cell = obj.GetComponent<GridCell>();
                if (cell)
                {
                    cell.Row = row;
                    cell.Column = column;

                    int idx = row * m_column + column;
                    m_gridCells[idx] = cell;
                }
            }
        }

        ///debug
        // string str = @"X|X|X|0|
        // X|X|X|<color=red>2</color>|
        // X|X|1|9|
        // X|X|X|9|";
        // GenerateUnitFromStr(str);
    }

    internal void SpawnOpponentAndKillUnitAt(int gridIdx, int level)
    {
        if (HasFlag(EGridFlag.Block, gridIdx))
        {
            //was blocked
            GameObject blockEffect = m_gridCells[gridIdx].BlockEffect;
            if (blockEffect)
            {
                blockEffect.SetActive(false);
            }
            m_gridCells[gridIdx].BlockEffect = null;

            return;
        }
        int row = gridIdx / m_column;
        int column = gridIdx % m_column;

        GridUnit unit = GetUnitAt(row, column);

        bool shouldBlock = unit != null && unit.UnitType == EUnitType.ALLY && m_skillData.ShouldBlockEnemyPortal();
        if (shouldBlock)
        {
            Debug.Log("Block enemy at pos " + row + ", " + column);
            return;
        }

        KillUnitAt(row, column);
        SpawnOpponentUnitAt(row, column, level);
    }

    private void KillUnitAt(int row, int column)
    {
        var unit = GetUnitAt(row, column);

        if (unit != null)
        {
            GameObject unitGO = unit.CurrentGO;
            unit.CurrentGO = null;

            unitGO.LeanScale(Vector3.one / 10, m_animTimeUnit);
            float y = unitGO.transform.position.y + 1;
            unitGO.LeanMoveY(y, m_animTimeUnit).setEaseOutCubic();
            Destroy(unitGO, m_animTimeUnit);

            SetUnitTo(row, column, null);

            SoundManager.PlaySFX(SoundDefine.k_SFX_destroy);
        }
    }

    public void KillOpponentUnitAt(int row, int column)
    {
        var unit = GetUnitAt(row, column);

        if (unit != null && unit.UnitType == EUnitType.OPPONENT)
        {
            GameObject unitGO = unit.CurrentGO;
            unit.CurrentGO = null;

            unitGO.LeanScale(Vector3.one / 10, m_animTimeUnit);
            Destroy(unitGO, m_animTimeUnit);

            var vfx = EffectManager.GetNextObjectWithName("disappear");
            if (vfx)
            {
                vfx.transform.position = unitGO.transform.position;
            }

            SetUnitTo(row, column, null);
            string str = ShowMatrixOnConsole();
            ShowDebug(str);
        }
    }

    public bool LevelUpUnitAt(int row, int column)
    {
        GridUnit unit = GetUnitAt(row, column);
        if (unit != null && unit.UnitType == EUnitType.ALLY)
        {
            if (unit.Level < GetMaxLevelUnit())
            {
                GameObject unitGO = unit.CurrentGO;
                int level = unit.Level + 1;

                SpawnAllyUnitAt(row, column, level);

                Destroy(unitGO);

                return true; // level up success
            }
        }
        return false;
    }

    public Vector3 ConvertGridToWorldPos(int row, int column)
    {
        float x = column * (m_blockSize + m_spacing) + (m_blockSize - m_gridWidth) / 2;
        float y = row * (m_blockSize + m_spacing) + (m_blockSize - m_gridHeight) / 2;
        return new Vector3(x, y, 0);
    }

    public Vector3 ConvertGridToWorldPos(int gridIdx)
    {
        int row = gridIdx / m_column;
        int column = gridIdx % m_column;

        return ConvertGridToWorldPos(row, column);
    }

    public GridUnit GetUnitAt(int row, int column)
    {
        return m_gridUnits[row * m_column + column];
    }

    void SetUnitTo(int row, int column, GridUnit unit)
    {
        int idx = row * m_column + column;
        m_gridUnits[idx] = unit;
    }

    ////////////////////////////////////////////////////////////////////////////////
    public GameObject SpawnAllyUnitAt(int row, int column, int level)
    {
        if (level > GetMaxLevelUnit())
        {
            level = GetMaxLevelUnit();
        }
        if (m_allyUnits[level])
        {
            Vector3 pos = ConvertGridToWorldPos(row, column);
            var go = Instantiate(m_allyUnits[level], pos, Quaternion.identity);

            var unit = GetUnitAt(row, column);
            if (unit == null)
            {
                unit = new GridUnit()
                {
                    UnitType = EUnitType.ALLY,
                    CurrentGO = go,
                    Level = level,
                };
                SetUnitTo(row, column, unit);
            }
            else
            {
                unit.CurrentGO = go;
                unit.UnitType = EUnitType.ALLY;
                unit.Level = level;
            }
            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            go.LeanScale(Vector3.one, m_animTimeUnit).setEaseOutBack();

            string str = ShowMatrixOnConsole();
            ShowDebug(str);

            return unit.CurrentGO;
        }
        return null;
    }
    public GameObject SpawnGiftUnitAt(int gridIdx)
    {
        int row = gridIdx / m_column;
        int column = gridIdx % m_column;

        return SpawnGiftUnitAt(row, column);
    }
    public GameObject SpawnGiftUnitAt(int row, int column)
    {
        if (m_giftUnit)
        {
            Vector3 pos = ConvertGridToWorldPos(row, column);
            var go = Instantiate(m_giftUnit, pos, Quaternion.identity);

            var unit = GetUnitAt(row, column);
            if (unit == null)
            {
                unit = new GridUnit()
                {
                    UnitType = EUnitType.GIFT,
                    CurrentGO = go,
                    Level = 0,
                };
                SetUnitTo(row, column, unit);
            }
            else
            {
                unit.CurrentGO = go;
                unit.UnitType = EUnitType.GIFT;
                unit.Level = 0;
            }

            unit.RemoveTime = Time.time + GameConst.k_time_delay_to_disable_gift_box;

            UnitGiftbox giftComponent = go.GetComponent<UnitGiftbox>();
            if (giftComponent)
            {
                giftComponent.GridPos = row * m_column + column;
            }
            else
            {
                Debug.LogWarning("Spawn unit Gift but not found component UnitGiftbox");
            }

            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            go.LeanScale(Vector3.one, m_animTimeUnit).setEaseOutBack();

            string str = ShowMatrixOnConsole();
            ShowDebug(str);

            return unit.CurrentGO;
        }
        return null;
    }

    public void CollectUnitGift(int gridIdx, bool isOpen)
    {
        int row = gridIdx / m_column;
        int column = gridIdx % m_column;

        var unit = GetUnitAt(row, column);
        if (unit != null && unit.UnitType == EUnitType.GIFT)
        {
            SetUnitTo(row, column, null); //remove gift out of grid if no unit overwrite on it

            string str = ShowMatrixOnConsole();
            ShowDebug(str);
        }

        if (isOpen)
        {
            OpenedGift = true;
        }
    }

    internal void HideAllConsiderPos()
    {
        // deactive all last consider positions 
        foreach (var item in m_preSpawnEffectList)
        {
            if (item.activeSelf)
            {
                item.SetActive(false);
            }
        }
    }
    internal void ShowConsiderPos(List<int> oppAppearPos)
    {

        for (int i = 0; i < oppAppearPos.Count; i++)
        {
            Vector3 pos = ConvertGridToWorldPos(oppAppearPos[i]);

            GameObject effect = GetInactiveObject(m_preSpawnEffectList);
            if (effect)
            {
                effect.SetActive(true);
                effect.transform.position = pos;
            }
        }
    }

    public GameObject SpawnOpponentUnitAt(int row, int column, int level)
    {
        if (level > GetMaxLevelUnit())
        {
            level = GetMaxLevelUnit();
        }
        if (m_oponentUnits[level])
        {
            Vector3 pos = ConvertGridToWorldPos(row, column);
            var go = Instantiate(m_oponentUnits[level], pos, Quaternion.identity);

            var unit = GetUnitAt(row, column);
            if (unit == null)
            {
                unit = new GridUnit()
                {
                    UnitType = EUnitType.OPPONENT,
                    CurrentGO = go,
                    Level = level,
                };
                SetUnitTo(row, column, unit);
            }
            else
            {
                unit.UnitType = EUnitType.OPPONENT;
                unit.CurrentGO = go;
                unit.Level = level;
            }

            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            go.LeanScale(Vector3.one, m_animTimeUnit).setEaseOutBack();

            string str = ShowMatrixOnConsole();
            ShowDebug(str);

            return unit.CurrentGO;
        }
        return null;
    }

    public void SpawnUnitRandom(EUnitType unitType, int level)
    {
        List<int> considerIds = new List<int>();

        for (int i = 0; i < m_gridUnits.Length; i++)
        {
            if (m_gridUnits[i] == null && !HasFlag(EGridFlag.Block, i))
            {
                considerIds.Add(i);
            }
        }

        if (considerIds.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, considerIds.Count);
            int row = considerIds[random] / m_column;
            int col = considerIds[random] % m_column;
            if (unitType == EUnitType.ALLY)
            {
                SpawnAllyUnitAt(row, col, level);
            }
            else if (unitType == EUnitType.OPPONENT)
            {
                SpawnOpponentUnitAt(row, col, level);
            }
        }

        SoundManager.PlaySFX(SoundDefine.k_SFX_appear);

    }

    public List<int> GetEmptyCell()
    {
        List<int> considerIds = new List<int>();

        for (int i = 0; i < m_gridUnits.Length; i++)
        {
            if (m_gridUnits[i] == null && !HasFlag(EGridFlag.Block, i))
            {
                considerIds.Add(i);
            }
        }
        return considerIds;
    }

    internal void SetGridConsiderPos(List<int> posList)
    {
        m_considerOpponentPos = posList;
    }
    #endregion
    #region Grid Unit move/merge
    public void MoveGridUnits(EDirection direction)
    {
        OpenedGift = false;

        //save state before move
        SaveState();

        List<MovementDetail> movements = null;
        if (direction == EDirection.Left || direction == EDirection.Right)
        {
            movements = MoveHorizontal(direction);
        }
        else if (direction == EDirection.Up || direction == EDirection.Down)
        {
            movements = MoveVertical(direction);
        }

        if (movements != null && movements.Count > 0)
        {
            IsMovingGrid = true;

            StartCoroutine(AnimateItems(movements));

            string str = ShowMatrixOnConsole();
            ShowDebug(str);
        }
    }

    public List<MovementDetail> MoveHorizontal(EDirection horizontalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<MovementDetail>();

        //the relative column we will compare with
        //if swipe is left, we will compare with the previous one (the -1 position)
        int relativeColumn = horizontalMovement == EDirection.Left ? -1 : 1;
        //to get the column indexes, to do the loop below
        var columnNumbers = Enumerable.Range(0, m_column);

        //for left swipe, we will traverse the columns in the order 0,1,2,3
        //for right swipe, we want the reverse order
        if (horizontalMovement == EDirection.Right)
        {
            columnNumbers = columnNumbers.Reverse();
        }

        for (int row = m_row - 1; row >= 0; row--)
        {   //we're doing foreach instead of for in order to traverse the columns
            //in the appropriate order
            foreach (int column in columnNumbers)
            {
                //if the item is null, continue checking for non-null items
                if (GetUnitAt(row, column) == null) continue;

                if (GetUnitAt(row, column).UnitType == EUnitType.GIFT) continue; //don't need to move gift

                //since we arrived here, we have a non-null item
                //first we check if this item has the same value as the previous one
                //previous one's position depends on whether the relativeColumn variable is -1 or 1, depending on the swipe
                MovementDetail imd = AreTheseTwoItemsSame(row, column, row, column + relativeColumn);
                if (imd != null)
                {
                    //items have the same value, so they will be "merged"
                    movementDetails.Add(imd);
                    //continue the loop
                    //the new duplicated item may be moved on a subsequent loop
                    continue;
                }

                MovementDetail mdAtk = AreTheseTwoItemsAttack(row, column, row, column + relativeColumn);
                if (mdAtk != null)
                {
                    movementDetails.Add(mdAtk);
                    continue;
                }

                //matrix[row,column] is the first not null item
                //move it to the first null item space
                int columnFirstNullItem = -1;

                //again, this is to help on the foreach loop that follows
                //for a left swipe, we want to check the columns 0 to [column-1]
                //for a right swipe, we want to check columns [m_column-1] to column
                int numberOfItemsToTake = horizontalMovement == EDirection.Left ? column : m_row - column;

                bool emptyItemFound = false;

                //keeping it for documentation/clarity
                //this for loop would run for a left swipe ;)
                //for (columnFirstNullItem = 0; columnFirstNullItem < column; columnFirstNullItem++)
                foreach (var tempColumnFirstNullItem in columnNumbers.Take(numberOfItemsToTake))
                {
                    //keep a copy of the index on the potential null item position
                    columnFirstNullItem = tempColumnFirstNullItem;

                    GridUnit unit = GetUnitAt(row, columnFirstNullItem);

                    //we consider GIFT unit is empty then other units can move to there
                    if (unit == null || unit.UnitType == EUnitType.GIFT)
                    {
                        emptyItemFound = true;
                        break;//exit the loop
                    }
                }

                //we did not find an empty/null item, so we cannot move current item
                if (!emptyItemFound)
                {
                    continue;
                }

                MovementDetail newImd = MoveItemToNullPositionAndCheckIfSameWithNextOne
                (row, row, row, column, columnFirstNullItem, columnFirstNullItem + relativeColumn);

                movementDetails.Add(newImd);

            }
        }
        return movementDetails;
    }

    public List<MovementDetail> MoveVertical(EDirection verticalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<MovementDetail>();

        int relativeRow = verticalMovement == EDirection.Down ? -1 : 1;
        var rowNumbers = Enumerable.Range(0, m_row);

        if (verticalMovement == EDirection.Up)
        {
            rowNumbers = rowNumbers.Reverse();
        }

        for (int column = 0; column < m_column; column++)
        {
            foreach (int row in rowNumbers)
            {
                //if the item is null, continue checking for non-null items
                if (GetUnitAt(row, column) == null) continue;

                if (GetUnitAt(row, column).UnitType == EUnitType.GIFT) continue; // don't need to move gift

                //we have a non-null item
                //first we check if this item has the same value as the next one
                MovementDetail imd = AreTheseTwoItemsSame(row, column, row + relativeRow, column);
                if (imd != null)
                {
                    movementDetails.Add(imd);

                    continue;
                }

                MovementDetail mdAtk = AreTheseTwoItemsAttack(row, column, row + relativeRow, column);
                if (mdAtk != null)
                {
                    movementDetails.Add(mdAtk);
                    continue;
                }

                //matrix[row,column] is the first not null item
                //move it to the first null item
                int rowFirstNullItem = -1;

                int numberOfItemsToTake = verticalMovement == EDirection.Down ? row : m_row - row;

                bool emptyItemFound = false;

                foreach (var tempRowFirstNullItem in rowNumbers.Take(numberOfItemsToTake))
                {
                    rowFirstNullItem = tempRowFirstNullItem;

                    GridUnit unit = GetUnitAt(rowFirstNullItem, column);

                    //we consider GIFT unit is empty then other units can move through there
                    if (unit == null || unit.UnitType == EUnitType.GIFT)
                    {
                        emptyItemFound = true;
                        break;
                    }
                }

                if (!emptyItemFound)
                {
                    continue;
                }

                MovementDetail newImd =
                MoveItemToNullPositionAndCheckIfSameWithNextOne(row, rowFirstNullItem, rowFirstNullItem + relativeRow, column, column, column);

                movementDetails.Add(newImd);
            }
        }
        return movementDetails;
    }

    private MovementDetail MoveItemToNullPositionAndCheckIfSameWithNextOne(int oldRow, int newRow, int itemToCheckRow, int oldColumn, int newColumn, int itemToCheckColumn)
    {
        //we found a null item, so we attempt the switch ;)
        //bring the first not null item to the position of the first null one
        SetUnitTo(newRow, newColumn, GetUnitAt(oldRow, oldColumn));
        SetUnitTo(oldRow, oldColumn, null);

        //check if we have the same value as the left one
        MovementDetail imd2 = AreTheseTwoItemsSame(newRow, newColumn, itemToCheckRow, itemToCheckColumn);
        if (imd2 != null) //we have, so add the item returned by the method
        {
            return imd2;
        }
        else
        {
            //check if have attack 
            MovementDetail mdAtk = AreTheseTwoItemsAttack(newRow, newColumn, itemToCheckRow, itemToCheckColumn);
            if (mdAtk != null)
            {
                return mdAtk;
            }
            else //they are not the same, so we'll just animate the current item to its new position
            {
                return
                    new MovementDetail(newRow, newColumn, GetUnitAt(newRow, newColumn).CurrentGO, null, null);

            }
        }
    }

    private MovementDetail AreTheseTwoItemsSame(int originalRow, int originalColumn, int toCheckRow, int toCheckColumn)
    {
        if (toCheckRow < 0 || toCheckColumn < 0 || toCheckRow >= m_row || toCheckColumn >= m_column)
            return null;

        var original = GetUnitAt(originalRow, originalColumn);
        var toCheck = GetUnitAt(toCheckRow, toCheckColumn);

        if (original != null && toCheck != null
                && original.Level == toCheck.Level
                && original.Level < GetMaxLevelUnit()
                && original.UnitType == toCheck.UnitType
                && !toCheck.WasJustDuplicated)
        {
            //double the value, since the item is duplicated
            toCheck.Level++;
            if (m_skillData.ShouldDoubleEvolve())
            {
                Debug.Log("double evolution from " + (toCheck.Level - 1) + " to " + (toCheck.Level + 1));
                toCheck.Level++;
                toCheck.WasDoubleEvolved = true;
            }
            toCheck.WasJustDuplicated = true;
            //make a copy of the gameobject to be moved and then disappear
            var GOToAnimateScaleCopy = original.CurrentGO;
            //remove this item from the array
            SetUnitTo(originalRow, originalColumn, null);
            return new MovementDetail(toCheckRow, toCheckColumn, toCheck.CurrentGO, GOToAnimateScaleCopy, null);
        }
        else
        {
            return null;
        }
    }

    private MovementDetail AreTheseTwoItemsAttack(int originalRow, int originalColumn, int toCheckRow, int toCheckColumn)
    {
        if (toCheckRow < 0 || toCheckColumn < 0 || toCheckRow >= m_row || toCheckColumn >= m_column)
            return null;

        var original = GetUnitAt(originalRow, originalColumn);
        var toCheck = GetUnitAt(toCheckRow, toCheckColumn);

        if (original != null && toCheck != null
            && original.UnitType != toCheck.UnitType
            && original.UnitType != EUnitType.GIFT
            && toCheck.UnitType != EUnitType.GIFT
            && !toCheck.WasJustDuplicated
        )
        {
            toCheck.WasJustDuplicated = true;

            var goToAnimateAtk = original.CurrentGO;
            bool killAllUnit = false;
            GridUnit unitBeKilled = null;
            if (original.Level > toCheck.Level)
            {
                EUnitType beKillType = toCheck.UnitType;
                toCheck.UnitType = original.UnitType; // change the unitType to unit has higher level
                if (m_attackReduceLevel)
                {
                    toCheck.Level = original.Level - toCheck.Level - 1; // reduce level
                }
                else
                {
                    int bekilledLv = toCheck.Level;
                    toCheck.Level = original.Level; //keep level of higher unit
                    goToAnimateAtk = toCheck.CurrentGO; //keep origin so need to destroy tocheck
                    toCheck.CurrentGO = original.CurrentGO;// switch CurrentGameObject to animate
                    killAllUnit = false;

                    //cache bekilled info to origin unit that pass its info to toCheck
                    original.UnitType = beKillType;
                    original.Level = bekilledLv;
                }
                unitBeKilled = original;
            }
            else if (original.Level < toCheck.Level)
            {
                if (m_attackReduceLevel)
                {
                    toCheck.Level = toCheck.Level - original.Level - 1;
                }
                else
                {
                    //keep level of higher unit (toCheck)
                    killAllUnit = false;
                }

                unitBeKilled = original;
            }
            else //2 unit equal
            {
                // set unit be killed to Opponent
                if (original.UnitType == EUnitType.OPPONENT)
                {
                    unitBeKilled = original;
                }
                else
                {
                    unitBeKilled = toCheck;
                }
                killAllUnit = true; // 2 unit kill together so no need spawn new unit

                if (m_skillData.ShouldSurviveWhenAttackSameLevelUnit())
                {

                    killAllUnit = false;
                    if (original.UnitType == EUnitType.ALLY)
                    {
                        // need to switch with Opponent at toCheck
                        toCheck.UnitType = original.UnitType; // change the unitType to unit has higher level
                        goToAnimateAtk = toCheck.CurrentGO; //keep origin so need to destroy tocheck
                        toCheck.CurrentGO = original.CurrentGO;// switch CurrentGameObject to animate
                        toCheck.WasJustSurvived = true;
                    }
                }
            }

            if (unitBeKilled != null && unitBeKilled.UnitType == EUnitType.OPPONENT)
            {
                // add score
                int score = m_scoreConfig.GetPointOfLevel(unitBeKilled.Level);
                GameplayController.Instance.AddScore(score);
                int gold = m_scoreConfig.GetGoldOfLevel(unitBeKilled.Level);
                gold += (int)m_skillData.bonusGoldPerKill;
                GameplayController.Instance.AddGold(gold);
            }

            SetUnitTo(originalRow, originalColumn, null);
            if (killAllUnit)
            {
                SetUnitTo(toCheckRow, toCheckColumn, null); //remove it from array
            }
            return new MovementDetail(toCheckRow, toCheckColumn, toCheck.CurrentGO, null, goToAnimateAtk, killAllUnit);
        }
        else
        {
            return null;
        }
    }

    private void ResetWasJustDuplicatedValues()
    {
        for (int row = 0; row < m_row; row++)
            for (int column = 0; column < m_column; column++)
            {
                var unit = GetUnitAt(row, column);
                if (unit != null && unit.WasJustDuplicated)
                {
                    unit.WasJustDuplicated = false;
                    unit.WasDoubleEvolved = false;
                    unit.WasJustSurvived = false;
                }
            }
    }

    private int GetMaxLevelUnit()
    {
        return m_allyUnits.Count - 1;
    }

    IEnumerator AnimateItems(IEnumerable<MovementDetail> movementDetails)
    {
        List<GameObject> objectsToDestroy = new List<GameObject>();
        float timewait = 0;
        foreach (var item in movementDetails)
        {
            //calculate the new position in the world space
            var newGoPosition = ConvertGridToWorldPos(item.NewRow, item.NewColumn);

            //move it there
            item.GOToAnimatePosition.LeanMove(newGoPosition, m_animTimeUnit).setEaseOutCubic();

            timewait = m_animTimeUnit;// Mathf.Max(timewait, m_animTimeUnit);

            //the scale is != null => this means that this item will also move and duplicate
            if (item.GOToAnimateScale != null)
            {
                var duplicatedItem = GetUnitAt(item.NewRow, item.NewColumn);

                // UpdateScore(duplicatedItem.Value);

                //check if the item is 2048 => game has ended
                // if (duplicatedItem.Level == GetMaxLevelUnit())
                // {
                //     //TODO: check if can't merge when item is max level
                //     yield return new WaitForEndOfFrame();
                // }

                // create the duplicated item and
                // assign it to the proper position in the array
                if (duplicatedItem.UnitType == EUnitType.ALLY)
                    SpawnAllyUnitAt(item.NewRow, item.NewColumn, duplicatedItem.Level);
                else
                {
                    SpawnOpponentUnitAt(item.NewRow, item.NewColumn, duplicatedItem.Level);
                }

                //we need two animations to happen in chain
                //first, the movement animation
                item.GOToAnimateScale.LeanMove(newGoPosition, m_animTimeUnit).setEaseOutCubic();
                //then, the scale down to minimals
                item.GOToAnimateScale.LeanScale(Vector3.one / 10, m_animTimeUnit)
                    .setDelay(m_animTimeUnit);


                item.GOToAnimatePosition.LeanScale(Vector3.one / 10, m_animTimeUnit)
                    .setDelay(m_animTimeUnit);

                //destroy objects after the animations have ended
                objectsToDestroy.Add(item.GOToAnimateScale);
                objectsToDestroy.Add(item.GOToAnimatePosition);

                //timewait = Mathf.Max(timewait, m_animTimeUnit * 1.5f);//2 actions

                SoundManager.PlaySFX(SoundDefine.k_SFX_merge);

            }
            if (item.GOToAnimateAttack != null)
            {
                var unit = GetUnitAt(item.NewRow, item.NewColumn);

                if (unit != null && m_attackReduceLevel && !item.ShouldKillAllUnits)
                {
                    // create the duplicated item and
                    // assign it to the proper position in the array
                    if (unit.UnitType == EUnitType.ALLY)
                        SpawnAllyUnitAt(item.NewRow, item.NewColumn, unit.Level);
                    else
                    {
                        SpawnOpponentUnitAt(item.NewRow, item.NewColumn, unit.Level);
                    }
                }

                item.GOToAnimateAttack.LeanMove(newGoPosition, m_animTimeUnit).setEaseOutCubic();
                //then, the scale down to minimals
                item.GOToAnimateAttack.LeanScale(Vector3.one / 10, m_animTimeUnit)
                    .setDelay(m_animTimeUnit);
                var vfx = EffectManager.GetNextObjectWithName("disappear");
                if (vfx)
                {
                    vfx.transform.position = newGoPosition;
                }

                if (m_attackReduceLevel || item.ShouldKillAllUnits)
                {
                    item.GOToAnimatePosition.LeanScale(Vector3.one / 10, m_animTimeUnit)
                       .setDelay(m_animTimeUnit);
                }

                objectsToDestroy.Add(item.GOToAnimateAttack);

                if (m_attackReduceLevel || item.ShouldKillAllUnits)
                {
                    objectsToDestroy.Add(item.GOToAnimatePosition);
                }

                //timewait = Mathf.Max(timewait, m_animTimeUnit);//2 actions

                SoundManager.PlaySFX(SoundDefine.k_SFX_destroy);
            }
        }

        // GamePlay will controll ally spawn with level
        // if (GameplayController.Instance.CurrentState == EGameState.PREPARE)
        // {
        //     SpawnUnitRandom(EUnitType.ALLY, 0);
        // }
        // else
        // {
        //     SpawnUnitRandom(EUnitType.OPPONENT, 0);
        // }
        //hold on till the animations finish
        yield return new WaitForSeconds(timewait / 2);

        IsMovingGrid = false;

        foreach (var go in objectsToDestroy)
        {
            Destroy(go);
            // go.SetActive(false);
        }
    }

    void SpawnConsiderOpponents()
    {
        for (int i = 0; i < m_considerOpponentPos.Count; i++)
        {
            int pos = m_considerOpponentPos[i];

            int row = pos / m_column;
            int column = pos % m_column;
            Vector3 worldPos = ConvertGridToWorldPos(pos);

            SpawnOpponentUnitAt(row, column, 0);
        }
    }
    #endregion
    #region Utils

    private void ShowDebug(string str)
    {
        if (m_debugText)
        {
            m_debugText.text = str;
        }
    }
    string ShowMatrixOnConsole()
    {
        string x = string.Empty;
        for (int row = m_row - 1; row >= 0; row--)
        {
            for (int column = 0; column < m_column; column++)
            {
                var unit = GetUnitAt(row, column);
                if (unit != null)
                {
                    if (unit.UnitType == EUnitType.ALLY)
                    {
                        x += unit.Level + "|";
                    }
                    else if (unit.UnitType == EUnitType.OPPONENT)
                    {
                        x += "<color=red>" + unit.Level + "</color>|";
                    }
                    else
                    {
                        x += "<color=green>" + unit.Level + "</color>|";
                    }
                }
                else
                {
                    x += "X" + "|";
                }
            }
            x += Environment.NewLine;
        }
        if (m_showConsole) Debug.Log(x);
        return x;
    }

    void GenerateUnitFromStr(string str)
    {
        /* 
        X|X|X|0|
        X|X|X|<color=red>2</color>|
        X|X|1|2|
        X|X|X|5|
         */

        var splits = str.Split('|');
        int row = m_row - 1;
        int column = 0;

        for (int i = 0; i < splits.Length; i++)
        {
            string s = splits[i];
            if (s.Equals("X"))
            {
                // skip;
            }
            else
            {
                if (s.Length == 1)
                {
                    int level = int.Parse(s);
                    SpawnAllyUnitAt(row, column, level);
                }
                else
                {
                    if (s.StartsWith("<color=red>"))
                    {
                        int level = int.Parse(s.Substring(11, 1));
                        SpawnOpponentUnitAt(row, column, level);
                    }
                }
            }

            column++;
            if (column >= m_column)
            {
                column = 0;
                row--;
            }
        }
    }

    private GameObject GetInactiveObject(List<GameObject> objects)
    {
        return objects.Find((obj) => { return obj.activeSelf == false; });
    }

    public bool HasEmptyCell()
    {
        var cells = GetEmptyCell();
        return cells.Count > 0;
    }

    public bool HasAllyUnit()
    {
        for (int i = 0; i < m_gridUnits.Length; i++)
        {
            if (m_gridUnits[i] != null && m_gridUnits[i].UnitType == EUnitType.ALLY)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasOpponentUnit()
    {
        for (int i = 0; i < m_gridUnits.Length; i++)
        {
            if (m_gridUnits[i] != null && m_gridUnits[i].UnitType == EUnitType.OPPONENT)
            {
                return true;
            }
        }
        return false;
    }

    internal void Clear()
    {
        HideAllConsiderPos();
        for (int i = 0; i < m_gridUnits.Length; i++)
        {
            if (m_gridUnits[i] != null)
            {
                if (m_gridUnits[i].CurrentGO)
                {
                    Destroy(m_gridUnits[i].CurrentGO);
                }
                m_gridUnits[i] = null;
            }

            m_gridFlags[i] = 0;
        }
    }

    void SetFlag(EGridFlag flag, int row, int column)
    {
        int idx = row * m_column + column;
        m_gridFlags[idx] |= flag;
    }

    void UnsetFlag(EGridFlag flag, int row, int column)
    {
        int idx = row * m_column + column;
        m_gridFlags[idx] &= ~flag;
    }

    bool HasFlag(EGridFlag flag, int row, int column)
    {
        int idx = row * m_column + column;
        return (m_gridFlags[idx] & flag) == flag;
    }

    bool HasFlag(EGridFlag flag, int idx)
    {
        return (m_gridFlags[idx] & flag) == flag;
    }

    public void ShowUnitInfo(bool isShow)
    {
        for (int i = 0; i < GridLength; i++)
        {
            if (m_gridUnits[i] != null)
            {
                UnitNormal unitObj = m_gridUnits[i].CurrentGO.GetComponentInChildren<UnitNormal>();
                if (unitObj != null)
                {
                    unitObj.ShowInfo(isShow);
                }
            }
        }
    }

    public int GetHighestAllyLevel()
    {
        int lv = 0;
        for (int i = 0; i < GridLength; i++)
        {
            if (m_gridUnits[i] != null && m_gridUnits[i].UnitType == EUnitType.ALLY)
            {
                lv = m_gridUnits[i].Level > lv ? m_gridUnits[i].Level : lv;
            }
        }
        return lv;
    }

    #endregion

    #region Grid state save/load
    void SaveState()
    {
        for (int i = 0; i < GridLength; i++)
        {
            GridUnit unit = m_gridUnits[i];
            if (unit == null)
            {
                m_gridState.SetCellState(i, EUnitType.ALLY, -1); //set level = -1 mean no unit at this pos
            }
            else
            {
                m_gridState.SetCellState(i, unit.UnitType, unit.Level);
            }
        }

        m_hasSavedState = true;
    }

    public void UndoLastMove()
    {
        IsMovingGrid = true;
        LoadState();
        m_hasSavedState = false;
        LeanTween.delayedCall(m_animTimeUnit, () =>
        {
            IsMovingGrid = false;
        });

        string str = ShowMatrixOnConsole();
        ShowDebug(str);
    }

    void LoadState()
    {
        if (!m_hasSavedState) return;
        for (int i = 0; i < GridLength; i++)
        {
            var cellState = m_gridState[i];
            GridUnit unit = m_gridUnits[i];
            int row = i / m_column;
            int column = i % m_column;

            if (cellState.IsEmpty)
            {
                if (unit != null)
                {
                    Destroy(unit.CurrentGO);
                    m_gridUnits[i] = null;
                }
            }
            else
            {
                if (unit == null)
                {
                    if (cellState.UnitType == EUnitType.ALLY)
                    {
                        SpawnAllyUnitAt(row, column, cellState.Level);
                    }
                    else if (cellState.UnitType == EUnitType.OPPONENT)
                    {
                        SpawnOpponentUnitAt(row, column, cellState.Level);
                    }
                }
                else //already have unit
                {
                    // check if unit is the same as last state
                    if (unit.UnitType == cellState.UnitType && unit.Level == cellState.Level)
                    {
                        //do nothing
                    }
                    else
                    {
                        Destroy(unit.CurrentGO); //delete current gameobj
                        if (cellState.UnitType == EUnitType.ALLY)
                        {
                            SpawnAllyUnitAt(row, column, cellState.Level);
                        }
                        else if (cellState.UnitType == EUnitType.OPPONENT)
                        {
                            SpawnOpponentUnitAt(row, column, cellState.Level);
                        }
                    }
                }
            }
        }
    }

    public bool CanUndoLastMove()
    {
        return m_hasSavedState;
    }
    #endregion
}
