using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPorteeTir : MonoBehaviour
{
    [SerializeField] private int m_portee = 2;
    
    public int Portee { get => m_portee; set => m_portee = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Drone drone = collision.gameObject.GetComponentInParent<Drone>();
        if (drone != null)
        {
            // temps = distance / vitesse
            drone.ShotDelay = Portee/drone.ShotSpeed;
            Destroy(gameObject);
        }
    }
}
