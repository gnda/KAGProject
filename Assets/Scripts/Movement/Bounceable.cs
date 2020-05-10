using System;
using UnityEngine;

public class Bounceable : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Wall") ||
            other.transform.GetComponentInParent<Enemy>())
        {
            HandleBounce();
        }
    }
    
    private void OnCollisionStay(Collision other)
    {
        if (other.transform.CompareTag("Wall") ||
            other.transform.GetComponentInParent<Enemy>())
        {
            HandleBounce();
        }
    }

    private void HandleBounce()
    {
        var movable = GetComponentInParent<Movable>();
        var rb = movable.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            var force = movable.currentForce;
            rb.velocity = Vector2.zero;

            rb.AddForce(-force, ForceMode2D.Impulse);
        }
    }
}