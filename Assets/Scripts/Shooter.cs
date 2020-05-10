﻿using System;
using System.Collections;
using Drone;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float shootSpeed = 12f;
    [SerializeField] private float shootDelay = 0.5f;
    [SerializeField] private GameObject bulletPrefab;
    
    private Transform originateFrom;

    public bool IsShooting { get; set; }

    private void Start()
    {
        originateFrom = GetComponentInParent<CanShoot>().transform;
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
        Transform transf = transform;
        Vector2 pos = transf.position;
        Vector2 endPos = towards - pos;
        
        IsShooting = true;
        
        GameObject bulletGO = Instantiate(bulletPrefab, 
            pos, transf.rotation);

        if (originateFrom != null)
        {
            bulletGO.GetComponent<Bullet>().Origin = originateFrom;
        }
        
        Rigidbody bulletRb = bulletGO.GetComponent<Rigidbody>();

        //distance = temps * vitesse
        bulletRb.velocity = endPos.normalized * shootSpeed;

        yield return new WaitForSeconds(shootDelay);
        
        IsShooting = false;
    }
}