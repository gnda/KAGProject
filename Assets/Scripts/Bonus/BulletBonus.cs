using System;
using UnityEngine;

public class BulletBonus : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float duration = 2f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var player = other.transform.GetComponent<Player>();
        

        if (player != null)
        {
            Debug.Log("hre");
            var shooter = player.GetComponentInChildren<Shooter>();

            if (shooter != null)
            {
                shooter.bulletPrefab = bulletPrefab;
            }
        }
    }
}