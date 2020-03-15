using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : Manager<HudManager>
{

    [Header("HudManager")]
    #region Labels & Values
    [Header("Texts")]
    [SerializeField] private Text m_TxtBestScore;
    [SerializeField] private Text m_TxtScore;
    [SerializeField] private Text m_TxtNLives;
    [SerializeField] private Text m_TxtNEnemiesLeftBeforeVictory;
    #endregion

    #region Manager implementation
    protected override IEnumerator InitCoroutine()
    {
        yield break;
    }
    #endregion

    #region Callbacks to GameManager events
    //????
    protected override void GameStatisticsChanged(GameStatisticsChangedEvent e)
    {
        m_TxtBestScore.text = e.eBestScore.ToString();
        m_TxtScore.text = e.eScore.ToString();
        m_TxtNLives.text = e.eNLives.ToString();
        m_TxtNEnemiesLeftBeforeVictory.text = e.eNEnemiesLeftBeforeVictory.ToString();
    }
    #endregion


    #region Monobehaviour Lifecycle
    private void Update()
    {
        m_TxtScore.text = GameManager.Instance.Players[0].Score.ToString();
    }
    #endregion
    
}
