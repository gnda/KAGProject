using System;
using System.Resources;
using UnityEngine;
using SDD.Events;

public class PlayerController : SimpleGameStateObserver
{
	private Movable _movable;
	private Rotatable _rotatable;
	private Shooter _shooter;
		
	private void Start()
	{
		_movable = GetComponent<Movable>();
		_rotatable = GetComponentInChildren<Rotatable>();
		_shooter = GetComponentInChildren<Shooter>();
	}

	private void Reset()
	{
		transform.rotation = Quaternion.identity;
	}

	// Update is called once per frame
	void FixedUpdate() {
		if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

		// Drone Keyboard Control
		float hInput = Input.GetAxis("Horizontal");
		float vInput = Input.GetAxis("Vertical");
		
		
		Vector2 deplacement = Vector2.zero;
		
		if (Math.Abs(vInput) > 0.01 || Math.Abs(hInput) > 0.01)
		{
			Transform transf = _movable.transform;
			
			Vector2 hori = Time.fixedDeltaTime * hInput * 10f * (Vector2) transf.right;
			Vector2 verti = Time.fixedDeltaTime * vInput *  10f * (Vector2) transf.up;
			
			if(Math.Abs(vInput) > 0.01)
			{
				deplacement += verti;
			}
			if(Math.Abs(hInput) > 0.01)
			{
				deplacement += hori;
			}
			
			_rotatable.Rotate((Vector2) transf.position + hori + verti);
			//_moveable.ReductFuel(1);
		}
		
		_movable.Move(deplacement);

		// Rotatable Element Mouse Control

		bool fire = Input.GetAxis("Fire1") > 0;
		Vector3 mousePosition = Input.mousePosition;

		if (Camera.main != null)
		{
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
		}

		Transform shooterTransf = _shooter.transform;
		Vector3 shooterPos = shooterTransf.position;
		
		Vector2 direction = new Vector2(
			mousePosition.x - shooterPos.x,
			mousePosition.y - shooterPos.y
		);

		shooterTransf.up = direction;
		
		if (fire)
		{
			_shooter.Shoot(mousePosition);
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

	protected override void OnDestroy()
	{
		if (GameManager.Instance.IsPlaying)
		{
			EventManager.Instance.Raise(new GameOverEvent());
		}
	}

	//Game state events
	protected override void GameMenu(GameMenuEvent e)
	{
		Reset();
	}
}