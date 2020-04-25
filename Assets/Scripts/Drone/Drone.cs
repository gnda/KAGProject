using System;
using System.Collections;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;
    private int m_fuel = 0;
    private int m_fuel_time = 0;
    public Frame Frame { get; private set; }
    public Turret Turret { get; private set; }


    #region Physics gravity
    [SerializeField] Vector3 m_Gravity;
    #endregion
    
    [SerializeField] private float m_ShotSpeed = 12f; // vitesse du shot
    [SerializeField] private float m_ShotDelay = 0.5f; //supprimé avant x time ?
    [SerializeField] private GameObject bulletPrefab;
    
    public bool IsShooting { get; set; }
    public int Fuel { get => m_fuel; set => m_fuel = value; }
    public int Fuel_time { get => m_fuel_time; set => m_fuel_time = value; }
    public float ShotSpeed { get => m_ShotSpeed; set => m_ShotSpeed = value; }
    public float ShotDelay { get => m_ShotDelay; set => m_ShotDelay = value; }

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

    public void ReductFuel(int value)
    {
        if(m_fuel_time == 10)
        {
            int soustraction = Fuel - value;
            if (soustraction > 0)
            {
                Fuel = Fuel - value;
            }
            else
            {
                Fuel = 0;
                //game over
            }
            m_fuel_time = 0;
        }
        else
        {
            m_fuel_time++;
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

        //distance = temps * vitesse
        bulletRb.velocity = endPos.normalized * ShotSpeed;

        
        yield return new WaitForSeconds(ShotDelay);
        
        IsShooting = false;
    }
}