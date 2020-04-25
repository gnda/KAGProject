using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusMegaScore : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            player.Score += 1000;
            Destroy(gameObject);
        }
    }
}
