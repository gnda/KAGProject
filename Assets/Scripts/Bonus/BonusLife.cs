using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusLife : MonoBehaviour
{
    [SerializeField] private int m_lifes = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            player.GetComponent<Explodable>().Life += m_lifes;
        }
    }
}
