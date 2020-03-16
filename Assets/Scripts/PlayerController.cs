using System;
using System.Resources;
using UnityEngine;
using SDD.Events;

public class PlayerController : SimpleGameStateObserver, IEventHandler{

	private Drone m_Drone;
	private Transform m_Transform;
	
	[SerializeField]
	private Transform m_SpawnPoint;
	[SerializeField]
	private float m_TranslationSpeed;

	protected override void Awake()
	{
		base.Awake();
		m_Drone = GetComponentInChildren<Drone>();
		m_Transform = m_Drone.transform;
	}

	private void Reset()
	{
		transform.position = m_SpawnPoint.position;
		transform.rotation = Quaternion.identity;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

		// Drone Keyboard Control
		
		float hInput = Input.GetAxis("Horizontal");
		float vInput = Input.GetAxis("Vertical");
		

		Vector2 deplacement = m_Drone.transform.position;
		
		if (Math.Abs(vInput) > 0.01 || Math.Abs(hInput) > 0.01)
		{
			Vector2 hori = Time.fixedDeltaTime * m_TranslationSpeed * hInput * (Vector2) m_Transform.right ;
			Vector2 verti = Time.fixedDeltaTime * m_TranslationSpeed * vInput * (Vector2) m_Transform.up ;
			
			if(Math.Abs(vInput) > 0.01)
			{
				deplacement += verti;
			}
			if(Math.Abs(hInput) > 0.01)
			{
				deplacement += hori;
			}
			
			m_Drone.RotateFrame(deplacement + hori + verti);
		}
		
		m_Drone.Move(deplacement);

		// Turret Mouse Control

		bool fire = Input.GetAxis("Fire1") > 0;
		Vector3 mousePosition = Input.mousePosition;

		if (Camera.main != null)
		{
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
		}

		Transform turretTransform = m_Drone.Turret.transform;
		Vector3 turretPos = turretTransform.position;
		
		Vector2 direction = new Vector2(
			mousePosition.x - turretPos.x,
			mousePosition.y - turretPos.y
		);

		turretTransform.up = direction;
		
		if (fire)
		{
			m_Drone.Shoot(mousePosition);
		};
	}

	private void OnTriggerEnter(Collider other)
	{
		if(GameManager.Instance.IsPlaying
			&& other.gameObject.CompareTag("Enemy"))
		{
			EventManager.Instance.Raise(new PlayerHasBeenHitEvent());
		}
	}
	
	//Game state events
	protected override void GameMenu(GameMenuEvent e)
	{
		Reset();
	}
}