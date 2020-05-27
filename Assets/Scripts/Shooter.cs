using System;
using System.Collections;
using Drone;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float shootSpeed = 12f;
    [SerializeField] public float shootDelay = 0.5f;
    [SerializeField] public GameObject bulletPrefab;
    
    Transform originateFrom;

    public bool IsShooting { get; set; }

    private void Start()
    {
        originateFrom = GetComponentInParent<CanShoot>().transform;
    }
    
    public void LookAt(Vector3 position)
    {
        Transform transf = transform;
        Vector3 pos = transf.position;

        Vector2 direction = new Vector2(
            position.x - pos.x,
            position.y - pos.y
        );

        transf.up = direction;
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
        Vector2 pos = GetComponentInChildren<Canon>().transform.position;
        Vector2 endPos = towards - pos;
        
        IsShooting = true;

        GameObject bulletGO = Instantiate(bulletPrefab, pos, transform.rotation, 
        LevelsManager.Instance.CurrentLevel.transform);

        if (originateFrom != null)
        {
            var bullet = bulletGO.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Origin = originateFrom;
                Rigidbody2D bulletRb = bulletGO.GetComponent<Rigidbody2D>();
                bulletRb.velocity = endPos.normalized * shootSpeed;
            }
            else
            {
                var bullets = bulletGO.GetComponentsInChildren<Bullet>();
                foreach (var b in bullets)
                {
                    b.Origin = originateFrom;
                    Rigidbody2D bulletRb = b.GetComponent<Rigidbody2D>();
                    bulletRb.velocity = endPos.normalized * shootSpeed;
                }
            }
        }

        yield return new WaitForSeconds(shootDelay);
        
        IsShooting = false;
    }
}