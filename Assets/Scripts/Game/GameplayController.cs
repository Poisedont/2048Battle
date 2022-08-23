using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine;

public class GameplayController : Singleton<GameplayController>
{
    #region Serialized fields
    [SerializeField] GameplayConfig m_gameConfig;
    [SerializeField] SkillData m_skillData;
    #endregion

    #region Private fields
    float m_prepareTime;
    EGameState m_currentState;
    private float m_currentTime;
    private int m_WaveCurrent;
    private int m_NumOpponentsInWave;
    private bool m_IsGiftShowedOnWave;
    private List<int> m_ListIdxModuleOppAppear;
    private bool m_waitForGridMove;
    private int m_score;
    private int m_gold;
    private bool m_wasSpawnGift;
    private int m_timeSpawnGift;
    private int m_battleTurn;
    private float m_lockingTouch;
    #endregion

    #region Properties
    public float PrepareRemain { get { return m_prepareTime - m_currentTime; } }
    public EGameState CurrentState { get { return m_currentState; } }
    public int Score { get { return m_score; } }
    public bool SkillActivated { get; set; }
    public bool Paused { get; private set; }
    public int CurrentWave { get { return m_WaveCurrent; } }
    public List<int> CurrentPortals { get { return m_ListIdxModuleOppAppear; } }
    #endregion

    #region Unity callbacks
    void Start()
    {
        m_gameConfig.LoadConfig();
        ChangeState(EGameState.STOPPED);

        m_ListIdxModuleOppAppear = new List<int>();

        if (!m_skillData)
        {
            Debug.LogError("Skill data must be set for GameplayController!!!");
        }
    }

    void Update()
    {
        UpdateState();
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////
    private void UpdateState()
    {
        switch (m_currentState)
        {
            case EGameState.PREPARE:
                {
                    if (Paused) return;

                    m_currentTime += Time.deltaTime;

                    // check locking touch time
                    if (m_lockingTouch > 0)
                    {
                        if (m_lockingTouch - m_currentTime < 0)
                        {
                            m_lockingTouch = 0;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (m_waitForGridMove && !GridManager.Instance.IsMovingGrid)
                    {
                        m_waitForGridMove = false;
                        SpawnAlly();
                        if (CheckGameOver()) return;
                    }

                    if (m_currentTime > m_prepareTime)
                    {
                        ChangeState(EGameState.BATTLE);
                    }
                    else if (m_prepareTime - m_currentTime < 1f)
                    {
                        GamePlayMenu.GetMenuInstance().ShowBattlePopup();
                        m_lockingTouch = 1.5f;
                    }

                }
                break;
            case EGameState.BATTLE:
                {
                    if (Paused) return;

                    m_currentTime += Time.deltaTime;

                    // check locking touch time
                    if (m_lockingTouch > 0)
                    {
                        if (m_lockingTouch - m_currentTime < 0)
                        {
                            m_lockingTouch = 0;
                        }
                        else
                        {
                            return;
                        }
                    }
                    // grid finish its moving animate
                    if (m_waitForGridMove && !GridManager.Instance.IsMovingGrid)
                    {
                        m_battleTurn--;
                        GamePlayMenu menu = GamePlayMenu.GetMenuInstance();
                        if (menu)
                        {
                            menu.ShowTurnRemain(m_battleTurn);
                        }

                        m_waitForGridMove = false;

                        // check gift and show if 
                        if (!m_wasSpawnGift && m_timeSpawnGift != -1)
                        {
                            if (m_NumOpponentsInWave <= m_timeSpawnGift)
                            {
                                SpawnGift();
                            }
                        }

                        if (m_wasSpawnGift && GridManager.Instance.OpenedGift)
                        {
                            //pause game
                            Paused = true;
                            ConfirmAdsMenu.Open();
                        }

                        // request Grid show new opponent
                        for (int i = 0; i < m_ListIdxModuleOppAppear.Count; i++)
                        {
                            int level = m_gameConfig.GetLevelOppnentInTurn(GetCurOpponentWaveIndex());
                            GridManager.Instance.SpawnOpponentAndKillUnitAt(m_ListIdxModuleOppAppear[i], level);
                        }
                        m_ListIdxModuleOppAppear.Clear();

                        // then generate new consider pos if still have amount of opponent to spawn
                        GridManager.Instance.HideAllConsiderPos();

                        if (CheckGameOver()) return;
                        if (m_NumOpponentsInWave <= 0)
                        {
                            if (m_battleTurn == 0 || !GridManager.Instance.HasOpponentUnit())
                            {
                                ChangeState(EGameState.PREPARE);
                                {
                                    menu.ShowPreparePopup();
                                    m_lockingTouch = 1.5f;
                                }
                            }
                        }
                        else
                        {
                            GenerateNextOpponentPos();

                            GridManager.Instance.ShowConsiderPos(m_ListIdxModuleOppAppear);
                        }
                    }
                }
                break;
        }

    }

    private void SpawnGift()
    {
        var emptyCell = GridManager.Instance.GetEmptyCell();
        if (emptyCell.Count > 0 && emptyCell.Count - m_ListIdxModuleOppAppear.Count > 0) //still have slot for gift
        {
            int randIdx = UnityEngine.Random.Range(0, emptyCell.Count);
            while (m_ListIdxModuleOppAppear.Contains(emptyCell[randIdx]))
            {
                randIdx = UnityEngine.Random.Range(0, emptyCell.Count);
            }

            int cellId = emptyCell[randIdx];

            GridManager.Instance.SpawnGiftUnitAt(cellId);

        }

        m_wasSpawnGift = true; //set by true even if spawn succes or not to avoid spam this function
    }

    void ChangeState(EGameState newState)
    {
        m_currentTime = 0; //reset state time
        m_currentState = newState;
        EnterState(newState);
    }

    private void EnterState(EGameState newState)
    {
        switch (newState)
        {
            case EGameState.PREPARE:
                {
                    GetInfoNextWave();
                    GamePlayMenu menu = GamePlayMenu.GetMenuInstance();
                    if (menu)
                    {
                        menu.ShowCountDown();
                        menu.HideTurnRemain();
                    }

                    SpawnAlly();

                }
                break;
            case EGameState.BATTLE:
                {
                    GamePlayMenu menu = GamePlayMenu.GetMenuInstance();
                    if (menu)
                    {
                        menu.HideCountDown();
                        menu.ShowTurnRemain(m_battleTurn);
                    }

                    // NOTE: gift setup
                    m_wasSpawnGift = false;
                    int rand = UnityEngine.Random.Range(0, 100);
                    if (rand < m_gameConfig.GetRateSpawnGift())
                    {
                        int maxOpponents = m_NumOpponentsInWave;
                        // due to m_NumOpponentsInWave reduce every time spawn opponent,
                        // so we set time spawn = number of opponent remain to spawn
                        m_timeSpawnGift = maxOpponents - UnityEngine.Random.Range(0, maxOpponents);
                        m_timeSpawnGift = maxOpponents;
                    }
                    else
                    {
                        m_timeSpawnGift = -1; //not spawn gift
                    }

                    GenerateNextOpponentPos();

                    GridManager.Instance.ShowConsiderPos(m_ListIdxModuleOppAppear);

                }
                break;
            case EGameState.STOPPED:
                {
                    Paused = true;
                }
                break;
        }
    }
    #region Touch Events
    public void OnDragEventHandler(EDirection direction)
    {
        if (Paused) return;
        if (SkillActivated) return;
        if (m_lockingTouch > 0) return;

        //if (GridManager.Instance.IsMovingGrid) return;

        GridManager.Instance.MoveGridUnits(direction);

        if (GridManager.Instance.IsMovingGrid)
        {
            m_waitForGridMove = true;
        }
    }

    #endregion

    public void StartGame()
    {
        m_score = 0;
        m_WaveCurrent = -1;
        Paused = false;

        PlayerManager.Instance.CurrentKillEnemies = 0;
        // if (m_currentTime == 0) //right enter state
        // {
        //     GridManager.Instance.SpawnUnitRandom(EUnitType.ALLY, 0);
        // }

        SkillController.Instance.OnStartGame();

        GamePlayMenu.GetMenuInstance().UpdateScoreText(m_score);
        ChangeState(EGameState.PREPARE);
    }
    ////////////////////////////////////////////////////////////////////////////////
    #region Wave info
    private void GetInfoNextWave()
    {
        m_WaveCurrent++;
        int opponentWaveIdx = GetCurOpponentWaveIndex();
        m_prepareTime = m_gameConfig.GetNextTimeForNewWave(opponentWaveIdx);
        m_NumOpponentsInWave = m_gameConfig.GetNumOpponentNextWave(opponentWaveIdx);
        if (m_WaveCurrent >= 0)
        {
            m_IsGiftShowedOnWave = false;
        }

        m_battleTurn = m_gameConfig.GetMaxTurn(opponentWaveIdx);

        m_prepareTime += m_skillData.timePrepareAdd;
        m_battleTurn += (int)m_skillData.turnBattleAdd;

        Debug.Log("Game :Wave= " + opponentWaveIdx + " :m_prepareTime= " + m_prepareTime + " :Time.time= " + (Time.time));
    }

    private int GetCurOpponentWaveIndex()
    {
        int waveStart = ChapterDB.Instance.GetChapterConfig(PlayerManager.Instance.MapSeleted).OpponentStartWave - 1;
        return m_WaveCurrent + waveStart;
    }
    private int GetCurAllyWaveIndex()
    {
        int waveStart = ChapterDB.Instance.GetChapterConfig(PlayerManager.Instance.MapSeleted).AllyStartWave - 1;
        return m_WaveCurrent + waveStart;
    }

    private void GenerateNextOpponentPos()
    {
        // if no more opponent, skip 
        if (m_NumOpponentsInWave <= 0)
        {
            return;
        }

        int numOpponent = m_gameConfig.GetNumOpponentInTurn(GetCurOpponentWaveIndex());
        if (numOpponent > m_NumOpponentsInWave)
            numOpponent = m_NumOpponentsInWave;

        m_NumOpponentsInWave -= numOpponent;

        m_ListIdxModuleOppAppear.Clear(); // clear list before generate new pos
        // add numOpponent to spawn next time
        for (int i = 0; i < numOpponent; i++)
        {
            int rand = UnityEngine.Random.Range(0, GridManager.Instance.GridLength);

            m_ListIdxModuleOppAppear.Add(rand);
        }

    }

    private void SetGridConsiderPos(List<int> posList)
    {
        GridManager.Instance.SetGridConsiderPos(posList);
    }

    internal void AddScore(int score)
    {
        PlayerManager.Instance.CurrentKillEnemies++;
        m_score += score;

        GamePlayMenu.GetMenuInstance().UpdateScoreText(m_score);
    }

    internal void AddGold(int gold)
    {
        m_gold += gold;
        PlayerManager.Instance.CurrentGold = m_gold;
    }

    private void SpawnAlly()
    {
        int lv = m_gameConfig.GetLevelSpawnAlly(GetCurAllyWaveIndex());

        if (m_skillData.ShouldAllyHigherAppear())
        {
            ++lv;
        }

        GridManager.Instance.SpawnUnitRandom(EUnitType.ALLY, lv);
    }

    internal void OnTouchUp(int row, int column)
    {
        if (SkillActivated)
        {
            var skill = SkillController.Instance.ActiveSkill;

            skill.Target = new SkillTarget()
            {
                GridRow = row,
                GridColumn = column
            };
        }
    }

    private void OnMouseUp()
    {
        if (SkillActivated)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var hit = Physics2D.Raycast(worldPos, Vector2.zero, 1, LayerMask.GetMask("Grid"));
            if (hit.collider)
            {
                GridCell cell = hit.collider.gameObject.GetComponent<GridCell>();
                if (cell)
                {
                    OnTouchUp(cell.Row, cell.Column);
                }
            }
        }
    }
    #endregion

    #region Game controll
    public void ResumeGame()
    {
        Paused = false;
    }

    public void PauseGame()
    {
        Paused = true;
    }

    public void StopGame()
    {
        ChangeState(EGameState.STOPPED);
    }

    bool CheckGameOver()
    {
        GridManager gridManager = GridManager.Instance;
        bool isExistEmpty = gridManager.HasEmptyCell();
        bool isExistAlly = gridManager.HasAllyUnit();
        bool isExistOpponent = gridManager.HasOpponentUnit();

        if (!isExistEmpty || !isExistAlly || (isExistOpponent && m_battleTurn == 0))
        {
            Debug.Log("game over isExistAlly: " + isExistAlly + ", isExistEmpty: " + isExistEmpty);
            ChangeState(EGameState.STOPPED);
            // 
            gridManager.Clear();
            SkillController.Instance.Clear();

            PlayerManager playerMgr = PlayerManager.Instance;

            if (m_score > playerMgr.Score)
            {
                playerMgr.Score = m_score;

#if UNITY_ANDROID
                if (PlayGamesPlatform.Instance.localUser.authenticated)
                {
                    // Note: make sure to add 'using GooglePlayGames'
                    PlayGamesPlatform.Instance.ReportScore(m_score,
                        GPGSIds.leaderboard_score,
                        (bool success) =>
                        {
                            Debug.Log(" Leaderboard update success: " + success);
                        });
                }
#endif
            }
            playerMgr.CurrentScore = m_score;
            playerMgr.CurrentWavePass = m_WaveCurrent;

            int chapter = playerMgr.MapSeleted;
            // TODO: should optimize this code
            if (chapter == 0)
            {
                if (m_WaveCurrent > playerMgr.BestWavePass)
                {
                    playerMgr.BestWavePass = m_WaveCurrent;
                }
            }
            else if (chapter == 1)
            {
                if (m_WaveCurrent > playerMgr.BestWavePass1)
                {
                    playerMgr.BestWavePass1 = m_WaveCurrent;
                }
            }
            else if (chapter == 2)
            {
                if (m_WaveCurrent > playerMgr.BestWavePass2)
                {
                    playerMgr.BestWavePass2 = m_WaveCurrent;
                }
            }

            playerMgr.SaveProfile();

            LeanTween.delayedCall(0.1f, () =>
            {
                if (m_battleTurn == 0)
                {
                    GamePlayMenu.GetMenuInstance().ShowOutOfTurnPopup();
                }
                else if (!isExistEmpty)
                {
                    GamePlayMenu.GetMenuInstance().ShowCannotMovePopup();
                }
                else if (!isExistAlly)
                {
                    GamePlayMenu.GetMenuInstance().ShowAllAlliesDead();
                }
                else
                {
                    ResultMenu.Open(); //over with no reason!!
                }
            });

            return true;
        }
        return false;
    }
    #endregion
}
