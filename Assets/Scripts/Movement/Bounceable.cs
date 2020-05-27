using System;
using UnityEngine;

public class Bounceable : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Wall"))
        {
            GetComponentInParent<Explodable>().Life--;
            HandleBounce();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.CompareTag("Wall"))
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
            var rotatable = movable.GetComponentInChildren<Rotatable>();
            Vector2 rotDir = rotatable.transform.rotation * Vector2.up;
            Vector2 force = movable.currentForce;

            rb.velocity = Vector2.zero;

            var rotX = Mathf.RoundToInt(rotDir.x);
            var rotY = Mathf.RoundToInt(rotDir.y);
            var frcX = Mathf.RoundToInt(force.normalized.x);
            var frcY = Mathf.RoundToInt(force.normalized.y);
            
            if (rotX == frcX && rotY == frcY)
            {
                rb.AddForce(-force * 0.5f, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.one * 6f * rotDir, ForceMode2D.Impulse);
            }
        }
    }
}