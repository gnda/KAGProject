using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform m_Origin;
    private int m_portee = 4;
    
    public Transform Origin { get; set; }
    public int Portee { get => m_portee; set => m_portee = value; }

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