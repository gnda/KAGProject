using System.Collections;
using UnityEngine;
using SDD.Events;
using System.Linq;
using UnityEngine.UI;

public enum GameState
{
	gameMenu,
	gamePlay,
	gameNextLevel,
	gamePause,
	gameOver,
	gameVictory,
	gameCredits
}

public class GameManager : Manager<GameManager> {
	[Header(("GeneralPrefabs"))]
	[SerializeField] public GameObject[] DronePrefabs;
	private int playerDroneIndex = 0;
	
	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();

		//MainMenuManager
		EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
		EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);
		EventManager.Instance.AddListener<NextLevelButtonClickedEvent>(NextLevelButtonClicked);
		EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
		EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
		EventManager.Instance.AddListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
        
		//Exit
		EventManager.Instance.AddListener<ExitButtonClickedEvent>(ExitButtonClicked);

		//Enemy
		EventManager.Instance.AddListener<EnemyHasBeenDestroyedEvent>(EnemyHasBeenDestroyed);

		//Score Item
		EventManager.Instance.AddListener<ScoreItemEvent>(ScoreHasBeenGained);

		//Level
		EventManager.Instance.AddListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
		EventManager.Instance.AddListener<AllEnemiesOfLevelHaveBeenDestroyedEvent>(AllEnemiesOfLevelHaveBeenDestroyed);

		//Player
		EventManager.Instance.AddListener<PlayerHasBeenHitEvent>(PlayerHasBeenHit);
	}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();

		//MainMenuManager
		EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
		EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClicked);
		EventManager.Instance.RemoveListener<NextLevelButtonClickedEvent>(NextLevelButtonClicked);
		EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
		EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
		EventManager.Instance.RemoveListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
        
		//Exit
		EventManager.Instance.RemoveListener<ExitButtonClickedEvent>(ExitButtonClicked);

		//Enemy
		EventManager.Instance.RemoveListener<EnemyHasBeenDestroyedEvent>(EnemyHasBeenDestroyed);

		//Score Item
		EventManager.Instance.RemoveListener<ScoreItemEvent>(ScoreHasBeenGained);

		//Level
		EventManager.Instance.RemoveListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
		EventManager.Instance.RemoveListener<AllEnemiesOfLevelHaveBeenDestroyedEvent>(AllEnemiesOfLevelHaveBeenDestroyed);

		//Player
		EventManager.Instance.RemoveListener<PlayerHasBeenHitEvent>(PlayerHasBeenHit);

	}
	#endregion

	#region Manager implementation
	
	protected override IEnumerator InitCoroutine()
	{
		Menu();
        EventManager.Instance.Raise(new GameStatisticsChangedEvent()
        {
            eBestScore = BestScore,
            eScore = 0,
            eNLives = 0,
            eNEnemiesLeftBeforeVictory = 0,
            eFuel = 0
		});
		yield break;
	}
	#endregion
	
	#region Time
	void SetTimeScale(float newTimeScale)
	{
		Time.timeScale = newTimeScale;
	}
	#endregion

	#region Game State
	private GameState m_GameState;
	public bool IsPlaying { get { return m_GameState == GameState.gamePlay; } }
	#endregion

	//LIVES
	#region Lives
	[Header("Settings")]
	[SerializeField]
	private int m_NStartLives;

	private int m_NLives;
	public int NLives { get { return m_NLives; } }
	void DecrementNLives(int decrement)
	{
		SetNLives(m_NLives - decrement);
	}
    void IncrementtNLives(int increment)
    {
        SetNLives(m_NLives + increment);
    }

    void SetNLives(int nLives)
	{
		m_NLives = nLives;
		EventManager.Instance.Raise(new GameStatisticsChangedEvent()
		{
			eBestScore = BestScore, 
			eScore = m_Score, 
			eNLives = m_NLives, 
			eNEnemiesLeftBeforeVictory = m_NEnemiesLeftBeforeVictory
		});
	}
    #endregion

    #region Fuel
    private int m_Fuel;
    public int Fuel
    {
        get { return m_Fuel; }
        set
        {
            m_Fuel = value;
        }
    }
    void IncrementFuel(int increment)
    {
        SetFuel(m_Fuel + increment);
    }

    void SetFuel(int p_fuel)
    {
        Fuel = p_fuel;
        EventManager.Instance.Raise(new GameStatisticsChangedEvent()
        {
            eFuel = m_Score
        });
    }
    #endregion

    #region Score
    private int m_Score;
	public int Score {
		get { return m_Score; }
		set
		{
			m_Score = value;
			BestScore = Mathf.Max(BestScore, value);
		}
	}

	public int BestScore
	{
		get
		{
			return PlayerPrefs.GetInt("BEST_SCORE", 0);
		}
		set
		{
			PlayerPrefs.SetInt("BEST_SCORE", value);
		}
	}

	void IncrementScore(int increment)
	{
		SetScore(m_Score + increment);
	}

	void SetScore(int score)
	{
		Score = score;
		EventManager.Instance.Raise(new GameStatisticsChangedEvent()
		{
			eBestScore = BestScore,
			eScore = m_Score,
			eNLives= m_NLives,
			eNEnemiesLeftBeforeVictory = m_NEnemiesLeftBeforeVictory,
            eFuel = m_Fuel
        });
	}
	#endregion

	#region Enemies to be destroyed
	[Header("Victory condition")]
	// Victory Condition : a certain number of enemies must be destroyed
	[SerializeField] private int m_NEnemiesToDestroyForVictory;
	private int m_NEnemiesLeftBeforeVictory;
	void DecrementNEnemiesLeftBeforeVictory(int decrement)
	{
		SetNEnemiesLeftBeforeVictory(m_NEnemiesLeftBeforeVictory - decrement);
	}
	void SetNEnemiesLeftBeforeVictory(int nEnemies)
	{
		m_NEnemiesLeftBeforeVictory = nEnemies;
		EventManager.Instance.Raise(new GameStatisticsChangedEvent()
		{
			eBestScore = BestScore,
			eScore = m_Score, 
			eNLives = m_NLives,
			eNEnemiesLeftBeforeVictory = m_NEnemiesLeftBeforeVictory,
            eFuel = m_Fuel
		});
	}
	#endregion

	#region Elements

	public GameObject AllCameras
	{
		get { return GameObject.Find("Cameras"); }
	}

	public Camera MainCamera
	{
		get { return GameObject.FindGameObjectWithTag("MainCamera").
			GetComponent<Camera>(); }
	}
	
	public RawImage MinimapBackground
	{
		get { return GameObject.FindGameObjectsWithTag("MinimapCamera").
			First(go => go.name == "Background").GetComponent<RawImage>(); }
	}
	
	public Camera MinimapCamera
	{
		get { return GameObject.FindGameObjectsWithTag("MinimapCamera").
			First(go => go.name == "MinimapCamera").GetComponent<Camera>(); }
	}

	public Player[] Players
	{
		get
		{
			return FindObjectsOfType<Player>();
		}
	}
	public Transform[] PlayerTransforms
	{
		get
		{
			return FindObjectsOfType<PlayerController>().Select(item=>item.transform).ToArray();
		}
	}
	
	public _3DCollision[] _3DCollisions => FindObjectsOfType<_3DCollision>();

	public GameObject GetSelectedDrone()
	{
		playerDroneIndex = playerDroneIndex >= 0 ? playerDroneIndex % 
		    DronePrefabs.Length : DronePrefabs.Length + playerDroneIndex;

		return DronePrefabs[playerDroneIndex];
	}

	#endregion

	#region Game flow & Gameplay
	//Game initialization
	void InitNewGame(int levelNumber)
	{
		SetScore(0);
		SetNLives(m_NStartLives);
		SetNEnemiesLeftBeforeVictory(m_NEnemiesToDestroyForVictory);
		
		m_GameState = GameState.gameNextLevel;
		EventManager.Instance.Raise(new GoToNextLevelEvent()
			{eLevelIndex = --levelNumber});
	}
	#endregion

	#region Callbacks to events issued by LevelManager
	private void LevelHasBeenInstantiated(LevelHasBeenInstantiatedEvent e)
	{
		SetTimeScale(1);
		m_GameState = GameState.gamePlay;
	}
	#endregion

	#region Callbacks to events issued by Player
	private void PlayerHasBeenHit(PlayerHasBeenHitEvent e)
	{
		DecrementNLives(1);

		if (m_NLives == 0)
		{

			Over();
		}
	}
	#endregion

	#region Callbacks to events issued by Score items
	private void ScoreHasBeenGained(ScoreItemEvent e)
	{
		//IncrementScore(e.eScore.Score);
	}
	#endregion

	#region Callbacks to events issued by Enemy
	private void EnemyHasBeenDestroyed(EnemyHasBeenDestroyedEvent e)
	{
	}
	#endregion

	#region Callbacks to events issued by Level
	private void AllEnemiesOfLevelHaveBeenDestroyed(AllEnemiesOfLevelHaveBeenDestroyedEvent e)
	{
	}
	#endregion

	#region Callbacks to Events issued by MenuManager
	private void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
	{
		Menu();
	}

	private void PlayButtonClicked(PlayButtonClickedEvent e)
	{
		Play(0);
	}

	private void NextLevelButtonClicked(NextLevelButtonClickedEvent e)
	{
		EventManager.Instance.Raise(new GoToNextLevelEvent());
	}

	private void ResumeButtonClicked(ResumeButtonClickedEvent e)
	{
		Resume();
	}

	private void EscapeButtonClicked(EscapeButtonClickedEvent e)
	{
		if(IsPlaying)
			Pause();
	}
	
	private void CreditsButtonClicked(CreditsButtonClickedEvent e)
	{
		Credits();
	}

	private void ExitButtonClicked(ExitButtonClickedEvent e)
	{
		Exit();
	}
	#endregion

	#region GameState methods
	private void Menu()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameMenu;
		//MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
		EventManager.Instance.Raise(new GameMenuEvent());
	}

	private void Play(int levelNumber)
	{
		m_GameState = GameState.gamePlay;
		//MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_MUSIC);
		EventManager.Instance.Raise(new GamePlayEvent());
		InitNewGame(levelNumber);
	}

	private void Pause()
	{
		SetTimeScale(0);
		m_GameState = GameState.gamePause;
		EventManager.Instance.Raise(new GamePauseEvent());
	}

	private void Resume()
	{
		SetTimeScale(1);
		m_GameState = GameState.gamePlay;
		EventManager.Instance.Raise(new GameResumeEvent());
	}

	private void Over()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameOver;
		SfxManager.Instance.PlaySfx(Constants.GAMEOVER_SFX);
		EventManager.Instance.Raise(new GameOverEvent());
	}

	private void Victory()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameVictory;
		SfxManager.Instance.PlaySfx(Constants.VICTORY_SFX);
		EventManager.Instance.Raise(new GameVictoryEvent());
	}
	
	private void Credits()
	{
		SetTimeScale(1);
		m_GameState = GameState.gameCredits;
		//MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
		EventManager.Instance.Raise(new GameCreditsEvent());
	}

	private void Exit()
	{
		SetTimeScale(0);
		Application.Quit();
	}
	#endregion
}
