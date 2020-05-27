using System;
using UnityEngine;

public class BulletBonus : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.GetComponentInChildren<Shooter>().bulletPrefab = bulletPrefab;
        }
    }
}