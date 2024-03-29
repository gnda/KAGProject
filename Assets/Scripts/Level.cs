﻿using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using IEventHandler = SDD.Events.IEventHandler;

public class Level : MonoBehaviour,IEventHandler
{

	[SerializeField] private Transform playerSpawnPosition;
	[SerializeField] public float timeLimit;
	List<Enemy> m_Enemies = new List<Enemy>();

	public void SubscribeEvents()
	{
		EventManager.Instance.AddListener<EnemyHasBeenDestroyedEvent>(EnemyHasBeenDestroyed);
	}

	public void UnsubscribeEvents()
	{
		EventManager.Instance.RemoveListener<EnemyHasBeenDestroyedEvent>(EnemyHasBeenDestroyed);
	}

	private void OnDestroy()
	{
		UnsubscribeEvents();
	}

	private void Awake()
	{
		SubscribeEvents();
	}

	private void Start()
	{
		transform.position = new Vector3(0,0,0f);
		//enemies
		m_Enemies = GetComponentsInChildren<Enemy>().ToList();
		
		LoadLevelElements();
	}

	private void LoadLevelElements()
	{
		GameObject droneGO = Instantiate(GameManager.Instance.GetSelectedDrone(), 
			playerSpawnPosition.position, Quaternion.identity, transform);
		
		droneGO.AddComponent<PlayerController>();
		droneGO.AddComponent<Player>();

		Camera mainCamera = GameManager.Instance.MainCamera;

		GameObject allCameras = GameManager.Instance.AllCameras;
		allCameras.GetComponent<Follow>().targetTransform = droneGO.transform;
	}
	
	void EnemyHasBeenDestroyed(EnemyHasBeenDestroyedEvent e)
	{
		m_Enemies.RemoveAll(item => item.Equals(null));
	}
}