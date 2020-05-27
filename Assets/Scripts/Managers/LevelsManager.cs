using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using Event = SDD.Events.Event;

public class LevelsManager : Manager<LevelsManager> {

	[Header("LevelsManager")]
	#region levels & current level management
	[SerializeField] GameObject[] m_LevelsPrefabs;
	public int mCurrentLevelIndex;
	private GameObject m_CurrentLevelGO;
	public Level CurrentLevel { get; set; }

	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		yield break;
	}
	#endregion

	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();
		EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
		EventManager.Instance.AddListener<RevealExitEvent>(RevealExit);
}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();
		EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
		EventManager.Instance.RemoveListener<RevealExitEvent>(RevealExit);
	}
	#endregion

	#region Level flow
	void Reset()
	{
		Destroy(m_CurrentLevelGO);
		m_CurrentLevelGO = null;
		mCurrentLevelIndex = -1;
	}

	void InstantiateLevel(int levelIndex)
	{
		levelIndex = Mathf.Max(levelIndex, 0) % m_LevelsPrefabs.Length;
		m_CurrentLevelGO = Instantiate(m_LevelsPrefabs[levelIndex]);
		CurrentLevel = m_CurrentLevelGO.GetComponent<Level>();
	}

	private IEnumerator GoToNextLevelCoroutine()
	{
		Destroy(m_CurrentLevelGO);
		while (m_CurrentLevelGO) yield return null;

		InstantiateLevel(mCurrentLevelIndex);
		MusicLoopsManager.Instance.PlayMusic(mCurrentLevelIndex + 1);

		EventManager.Instance.Raise(new LevelHasBeenInstantiatedEvent() {eLevel = CurrentLevel});
	}
	#endregion

	#region Callbacks to GameManager events
	protected override void GameMenu(GameMenuEvent e)
	{
		Reset();
	}
	protected override void GamePlay(GamePlayEvent e)
	{
		Reset();
	}

	private void GoToNextLevel(GoToNextLevelEvent e)
	{
		mCurrentLevelIndex++;
		if (mCurrentLevelIndex > m_LevelsPrefabs.Length - 1)
		{
			EventManager.Instance.Raise(new TriggerGameVictoryEvent());
		}
		else
		{
			StartCoroutine(GoToNextLevelCoroutine());
		}
	}

	private void RevealExit(RevealExitEvent e)
	{
		m_CurrentLevelGO.transform.
			GetComponentInChildren<Exit>(true).gameObject.SetActive(true);
		var messages = CurrentLevel.GetComponentInChildren<Messages>();
		messages.transform.Find("Exit").gameObject.SetActive(true);
	}
	#endregion
}
