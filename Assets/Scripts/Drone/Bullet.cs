using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform m_Origin;
    
    public Transform Origin { get; set; }
    
    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}