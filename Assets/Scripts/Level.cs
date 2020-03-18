using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using System.Linq;
using UnityEngine.UIElements;
using IEventHandler = SDD.Events.IEventHandler;

public class Level : MonoBehaviour,IEventHandler
{

	[SerializeField] private Transform playerSpawnPosition;
	[SerializeField] private Color backgroundColor;
	
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
		//enemies
		m_Enemies = GetComponentsInChildren<Enemy>().ToList();
		
		GenerateLevel();
	}

	private void GenerateLevel()
	{
		GameObject playerGO = new GameObject("Player");
		playerGO.AddComponent<PlayerController>();
		playerGO.AddComponent<Player>();

		GameObject droneGO = Instantiate(GameManager.Instance.GetSelectedDrone(), 
			playerSpawnPosition.position, Quaternion.identity, playerGO.transform);
		
		GameManager.Instance.VirtualCamera.Follow = droneGO.transform;
		GameManager.Instance.MainCamera.backgroundColor = backgroundColor;
	}
	
	void EnemyHasBeenDestroyed(EnemyHasBeenDestroyedEvent e)
	{
		m_Enemies.RemoveAll(item => item.Equals(null));
	}
}