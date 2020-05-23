#region GameManager Events
public class GameMenuEvent : SDD.Events.Event
{
}
public class GamePlayEvent : SDD.Events.Event
{
}
public class GameInstructionsEvent : SDD.Events.Event
{
}
public class GamePauseEvent : SDD.Events.Event
{
}
public class GameResumeEvent : SDD.Events.Event
{
}
public class GameOverEvent : SDD.Events.Event
{
}
public class GameVictoryEvent : SDD.Events.Event
{
}
public class GameCreditsEvent : SDD.Events.Event
{
}
public class GameExitEvent : SDD.Events.Event
{
}
public class GameStatisticsChangedEvent : SDD.Events.Event
{
	public int eBestScore { get; set; }
	public int eScore { get; set; }
	public int eNLives { get; set; }
	public int eNEnemiesLeftBeforeVictory { get; set; }
    public int eFuel { get; set; }

}
#endregion

#region MenuManager Events
public class EscapeButtonClickedEvent : SDD.Events.Event
{
}
public class NextLevelButtonClickedEvent : SDD.Events.Event
{
}
public class PlayButtonClickedEvent : SDD.Events.Event
{
}
public class InstructionsButtonClickedEvent : SDD.Events.Event
{
}
public class ResumeButtonClickedEvent : SDD.Events.Event
{
}
public class MainMenuButtonClickedEvent : SDD.Events.Event
{
}
public class CreditsButtonClickedEvent : SDD.Events.Event
{
}
public class ExitButtonClickedEvent : SDD.Events.Event
{
}
#endregion

#region Enemy Event
public class EnemyHasBeenDestroyedEvent : SDD.Events.Event
{
	public bool eDestroyedByPlayer;
}
#endregion

#region Score Event
public class ScoreItemEvent : SDD.Events.Event
{
    public int eScore = 0;
}
#endregion

#region Score Event
public class LifeItemEvent : SDD.Events.Event
{
    public int eNLives = 0;
}
#endregion

#region Fuel Event
public class FuelItemEvent : SDD.Events.Event
{
    public int eFuel = 0;
}
#endregion

#region Game Manager Additional Event
public class AskToGoToNextLevelEvent : SDD.Events.Event
{
}
public class GoToNextLevelEvent : SDD.Events.Event
{
	public int eLevelIndex;
}
#endregion

#region Level Events
public class AllEnemiesOfLevelHaveBeenDestroyedEvent : SDD.Events.Event
{
}
#endregion

#region LevelsManager Events
public class LevelHasBeenInstantiatedEvent : SDD.Events.Event
{
	public Level eLevel;
}
#endregion

#region Player
public class PlayerHasBeenHitEvent:SDD.Events.Event
{
}
#endregion
