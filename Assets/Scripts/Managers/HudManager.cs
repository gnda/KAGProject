using System.Collections;
using SDD.Events;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : Manager<HudManager>
{

    [Header("HudManager")]
    #region Labels & Values
    [SerializeField] private GameObject panelHUD;
    
    [Header("Texts")]
    [SerializeField] private Text m_TxtBestScore;
    [SerializeField] private Text m_TxtScore;
    [SerializeField] private Text m_TxtNLives;
    [SerializeField] private Text m_TxtNEnemiesLeftBeforeVictory;
    [SerializeField] private Text m_TxtFuel;
    #endregion

    #region Manager implementation
    protected override IEnumerator InitCoroutine()
    {
        panelHUD.SetActive(false);
        yield break;
    }
    #endregion

    #region Monobehaviour Lifecycle
    private void Update()
    {
        if (GameManager.Instance.IsPlaying && GameManager.Instance.Players.Length > 0)
        {
            m_TxtScore.text = GameManager.Instance.Players[0].Score.ToString();
            m_TxtFuel.text = GameManager.Instance.Players[0].gameObject.GetComponentInChildren<Drone>().Fuel.ToString();
            m_TxtNLives.text = GameManager.Instance.Players[0].Life.ToString();
        }
    }
    #endregion

    #region Callbacks to GameManager events
    protected override void GameStatisticsChanged(GameStatisticsChangedEvent e)
    {
        m_TxtBestScore.text = e.eBestScore.ToString();
        m_TxtScore.text = e.eScore.ToString();
        m_TxtNLives.text = e.eNLives.ToString();
        m_TxtNEnemiesLeftBeforeVictory.text = e.eNEnemiesLeftBeforeVictory.ToString();
        m_TxtFuel.text = e.eFuel.ToString();
    }
    #endregion

    #region Events subscription
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();

        //level
        EventManager.Instance.AddListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
        EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
		
    }
    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();

        //level
        EventManager.Instance.RemoveListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
        EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);

    }
    #endregion
    
    #region Callbacks to Level events
    private void LevelHasBeenInstantiated(LevelHasBeenInstantiatedEvent e)
    {
        m_TxtBestScore.text = "--";
        m_TxtScore.text = "--";
        m_TxtNLives.text = "--";
        m_TxtNEnemiesLeftBeforeVictory.text = "--";
        m_TxtFuel.text = "--";
    }
	
    private void GoToNextLevel(GoToNextLevelEvent e){
        panelHUD.SetActive(true);
    }
    #endregion

    #region Callbacks to MenuManager events
    protected override void GameMenu(GameMenuEvent e)
    {
        if (panelHUD.activeInHierarchy){
            panelHUD.SetActive(false);
        }
    }

    protected override void GameCredits(GameCreditsEvent e){
        panelHUD.SetActive(false);
    }
    #endregion
}
