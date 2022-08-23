public class GridState
{
    ////////////////////////////////////////////////////////////////////////////////

    public class CellState
    {
        public EUnitType UnitType { get; set; }
        public int Level { get; set; }
        public bool IsEmpty { get { return Level < 0; } }
    }
    ////////////////////////////////////////////////////////////////////////////////
    CellState[] m_cellStates;
    private GridState() { }
    public GridState(int leng)
    {
        m_cellStates = new CellState[leng];

        for (int i = 0; i < leng; i++)
        {
            m_cellStates[i] = new CellState()
            {
                Level = -1,
            };
        }
    }
    public CellState this[int index]
    {
        get { return m_cellStates[index]; }
    }

    public void SetCellState(int idx, EUnitType type, int level)
    {
        m_cellStates[idx].UnitType = type;
        m_cellStates[idx].Level = level;
    }

}