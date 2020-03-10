using System;
using UnityEngine;
using SDD.Events;

public class PlayerController : SimpleGameStateObserver, IEventHandler{

	[Header("Spawn")]
	[SerializeField]
	private Transform m_SpawnPoint;

	#region Physics gravity
	[SerializeField] Vector3 m_LowGravity;
	[SerializeField] Vector3 m_HighGravity;
	#endregion


	private Drone m_Drone;
	private Rigidbody2D m_Rigidbody;
	private Transform m_Transform;

	[SerializeField]
	private float m_TranslationSpeed;
	[SerializeField]
	private float m_JumpImpulsionMagnitude;

	private bool m_IsGrounded;


	protected override void Awake()
	{
		base.Awake();
		m_Drone = GetComponentInChildren<Drone>();
		m_Rigidbody = m_Drone.GetComponent<Rigidbody2D>();
		m_Transform = GetComponent<Transform>();
	}

	private void Reset()
	{
		m_Rigidbody.position = m_SpawnPoint.position;
		m_Rigidbody.velocity = Vector2.zero;
		m_Rigidbody.angularVelocity = 0;
	}

	// Use this for initialization
	void Start () {
		m_IsGrounded = false;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

		float hInput = Input.GetAxis("Horizontal");
		float vInput = Input.GetAxis("Vertical");
		//bool jump = Input.GetAxis("Jump") > 0 || Input.GetKeyDown(KeyCode.Space);
		bool fire = Input.GetAxis("Fire1") > 0;

		// m_Rigidbody.rotation = Quaternion.AngleAxis(90 * Mathf.Sign(hInput), Vector2.up);
		Vector2 deplacement = m_Rigidbody.position;
		Vector2 hori = Time.fixedDeltaTime * m_TranslationSpeed * hInput * (Vector2) m_Transform.right ;
		Vector2 verti = Time.fixedDeltaTime * m_TranslationSpeed * vInput * (Vector2) m_Transform.up ;
		if(Math.Abs(vInput) > 0.01)
		{
			deplacement += verti;
			//StartCoroutine(m_Drone.RotationCoroutine(Vector2.up * Math.Sign(vInput)));
		}
		if(Math.Abs(hInput) > 0.01)
		{
			deplacement += hori;
			//StartCoroutine(m_Drone.RotationCoroutine(Vector2.right * Math.Sign(hInput)));
		}
		m_Rigidbody.MovePosition(deplacement);
		/*if (jump && m_IsGrounded)
		{
			Vector2 jumpForce = Vector2.up * m_JumpImpulsionMagnitude;
			m_Rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
		}*/

		if (m_IsGrounded)
		{
			m_Rigidbody.velocity = Vector2.zero;
		}

		m_Rigidbody.angularVelocity = 0;

		Vector2 gravity = m_HighGravity;
		if (fire && m_Rigidbody.velocity.y < 0) gravity = m_LowGravity;

		m_Rigidbody.AddForce(gravity*m_Rigidbody.mass);

	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground")
			|| collision.gameObject.CompareTag("Platform"))
		{
			Vector2 colLocalPt = m_Transform.InverseTransformPoint(collision.contacts[0].point);

			if (colLocalPt.magnitude<.5f)
				m_IsGrounded = true;
		}

	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground")
			|| collision.gameObject.CompareTag("Platform"))
		{
			m_IsGrounded = false;
		}
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