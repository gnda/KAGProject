using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusVitesseTir : MonoBehaviour
{
    [SerializeField] private float m_vitesse = 2.2f;

    public float Vitesse { get => m_vitesse; set => m_vitesse = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Drone.Drone drone = collision.gameObject.GetComponentInParent<Drone.Drone>();
        if (drone != null)
        {
            // temps = distance / vitesse
            //drone.ShotSpeed = Vitesse;
            Destroy(gameObject);
        }
    }
}
