using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayMenu : MenuBase<GamePlayMenu>
{
    [SerializeField] GameObject m_countDown;
    [SerializeField] GameObject m_turnCount;
    [SerializeField] Text m_bestScoreText;
    [SerializeField] Text m_scoreText;
    [SerializeField] Text m_waveText;
    [SerializeField] Text m_turnText;
    [SerializeField] GameObject m_battlePopup;
    [Tooltip("Failed reason: Out of turn")]
    [SerializeField] GameObject m_outOfTurn;
    [Tooltip("Failed reason: Can not move")]
    [SerializeField] GameObject m_cantMove;
    [Tooltip("Failed reason: All allies dead")]
    [SerializeField] GameObject m_allAllyDead;
    [SerializeField] GameObject m_skillPanel;


    private Text m_countDownText;
    private bool m_gameOver;
    void Start()
    {
        if (m_countDown)
        {
            m_countDownText = m_countDown.GetComponentInChildren<Text>();
        }
    }

    void Update()
    {
        if (m_countDown.activeSelf)
        {
            UpdateCountDown();
        }

        if (m_gameOver)
        {
            if (Input.GetMouseButtonUp(0))
            {
                ResultMenu.Open();
            }
        }
    }

    public override void OnBack()
    {
        GameplayController.Instance.PauseGame();
        IGMMenu.Open();
    }

    void UpdateCountDown()
    {
        float remainTime = GameplayController.Instance.PrepareRemain;
        if (m_countDownText)
        {
            m_countDownText.text = ((int)remainTime).ToString();
        }
    }

    public void HideCountDown()
    {
        if (m_countDown)
        {
            m_countDown.SetActive(false);
        }
    }

    public void ShowCountDown()
    {
        if (m_countDown)
        {
            m_countDown.SetActive(true);
        }

        if (m_waveText)
        {
            m_waveText.text = (GameplayController.Instance.CurrentWave + 1).ToString();
        }
    }

    public void UpdateScoreText(int score)
    {

        if (score > PlayerManager.Instance.Score)
        {
            m_bestScoreText.text = score.ToString();
        }

        if (m_scoreText)
        {
            m_scoreText.text = score.ToString();
        }
    }

    public void ShowBattlePopup()
    {
        if (m_battlePopup && !m_battlePopup.activeSelf)
        {
            m_battlePopup.SetActive(true);

            // NOTE: dkm cheat vl
            m_battlePopup.transform.GetChild(0).gameObject.SetActive(true); //0 is battle
            m_battlePopup.transform.GetChild(1).gameObject.SetActive(false); // 1 is prepare

            m_battlePopup.transform.position = new Vector3(-Screen.width, m_battlePopup.transform.position.y, m_battlePopup.transform.position.z);

            m_battlePopup.LeanMoveLocalX(0, 0.5f).setEaseOutCirc()
                    .setOnComplete(() =>
                    {
                        m_battlePopup.LeanDelayedCall(0.5f, () =>
                        {
                            float x = m_battlePopup.transform.position.x + Screen.width * 2;
                            m_battlePopup.LeanMoveLocalX(x, 0.5f).setEaseOutCirc()
                                .setOnComplete(() =>
                                {
                                    m_battlePopup.SetActive(false);
                                });
                        });
                    });
        }
    }

    public void ShowPreparePopup()
    {
        if (m_battlePopup && !m_battlePopup.activeSelf)
        {
            m_battlePopup.SetActive(true);

            m_battlePopup.transform.GetChild(0).gameObject.SetActive(false);
            m_battlePopup.transform.GetChild(1).gameObject.SetActive(true);

            m_battlePopup.transform.position = new Vector3(-Screen.width, m_battlePopup.transform.position.y, m_battlePopup.transform.position.z);

            m_battlePopup.LeanMoveLocalX(0, 0.5f).setEaseOutCirc()
                    .setOnComplete(() =>
                    {
                        m_battlePopup.LeanDelayedCall(0.5f, () =>
                        {
                            float x = m_battlePopup.transform.position.x + Screen.width * 2;
                            m_battlePopup.LeanMoveLocalX(x, 0.5f).setEaseOutCirc()
                                .setOnComplete(() =>
                                {
                                    m_battlePopup.SetActive(false);
                                });
                        });
                    });
        }
    }

    public void ShowSkillPanel()
    {
        m_skillPanel.SetActive(true);
    }

    protected override void OnMenuClosing()
    {
        m_skillPanel.SetActive(false);
        HideAllReasonPopups();
        base.OnMenuClosing();
    }

    protected override void OnMenuOpening()
    {
        base.OnMenuOpening();
        m_skillPanel.SetActive(true);
        m_bestScoreText.text = PlayerManager.Instance.Score.ToString();
        m_gameOver = false;
    }

    public void OnPauseBtnCLick()
    {
        GameplayController.Instance.PauseGame();
        IGMMenu.Open();
    }

    public void ShowTurnRemain(int turnRemain)
    {
        if (m_turnCount)
        {
            m_turnCount.SetActive(true);
        }
        if (m_turnText)
        {
            m_turnText.text = turnRemain.ToString();
        }
    }

    public void HideTurnRemain()
    {
        if (m_turnCount)
        {
            m_turnCount.SetActive(false);
        }
    }

    public void OnBattleInfoBtnDown()
    {
        GameplayController.Instance.PauseGame();
        GridManager.Instance.ShowUnitInfo(true);
    }

    public void OnBattleInfoBtnUp()
    {
        GameplayController.Instance.ResumeGame();
        GridManager.Instance.ShowUnitInfo(false);
    }

    public void ShowOutOfTurnPopup()
    {
        if (m_outOfTurn && !m_outOfTurn.activeSelf)
        {
            m_outOfTurn.SetActive(true);

            m_gameOver = true;
        }
    }

    public void ShowCannotMovePopup()
    {
        if (m_cantMove && !m_cantMove.activeSelf)
        {
            m_cantMove.SetActive(true);
            m_gameOver = true;
        }
    }
    public void ShowAllAlliesDead()
    {
        if (m_allAllyDead && !m_allAllyDead.activeSelf)
        {
            m_allAllyDead.SetActive(true);
            m_gameOver = true;
        }
    }
    private void HideAllReasonPopups()
    {
        if (m_outOfTurn && m_outOfTurn.activeSelf)
        {
            m_outOfTurn.SetActive(false);
        }
        if (m_cantMove && m_cantMove.activeSelf)
        {
            m_cantMove.SetActive(false);
        }
        if (m_allAllyDead && m_allAllyDead.activeSelf)
        {
            m_allAllyDead.SetActive(false);
        }
    }
}
