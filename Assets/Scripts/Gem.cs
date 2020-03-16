using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class Gem : MonoBehaviour
{
    [SerializeField] private int m_value;
    [SerializeField] private Color m_color;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            player.Score += m_value;
            Destroy(gameObject);
        }
    }
}
