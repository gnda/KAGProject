using System;
using System.Collections;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;
    public Frame Frame { get; private set; }
    public Turret Turret { get; private set; }

    #region Physics gravity
    [SerializeField] Vector3 m_Gravity;
    #endregion
    
    [SerializeField] private float m_ShotSpeed = 12f;
    [SerializeField] private float m_ShotDelay = 0.5f;
    [SerializeField] private GameObject bulletPrefab;
    
    public bool IsShooting { get; set; }

    private void Start()
    {
        Frame = GetComponentInChildren<Frame>();
        Turret = GetComponentInChildren<Turret>();
            
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.angularVelocity = 0;
    }

    public void Move(Vector2 endPos)
    {
        m_Rigidbody.MovePosition(endPos);
        m_Rigidbody.angularVelocity = 0;
        m_Rigidbody.AddForce(m_Gravity * m_Rigidbody.mass);
    }
    
    public void RotateFrame(Vector2 endPos)
    {
        SmoothRotation smr = Frame.GetComponentInChildren<SmoothRotation>();
        if (!smr.IsRotating)
        {
            StartCoroutine(smr.RotationCoroutine(endPos));
        }
    }

    public void Shoot(Vector2 towards)
    {
        if (!IsShooting)
        {
            StartCoroutine(ShootCoroutine(towards));    
        }
    }

    IEnumerator ShootCoroutine(Vector2 towards)
    {
        Vector2 turretPos = Turret.transform.position;
        Vector2 endPos = towards - turretPos;
        
        IsShooting = true;
        
        GameObject bulletGO = Instantiate(bulletPrefab, 
            turretPos, Turret.transform.rotation);
        bulletGO.GetComponent<Bullet>().Origin = transform;
        Rigidbody2D bulletRb = bulletGO.GetComponent<Rigidbody2D>();
        bulletRb.velocity = endPos.normalized * m_ShotSpeed;
        
        yield return new WaitForSeconds(m_ShotDelay);
        
        IsShooting = false;
    }
}