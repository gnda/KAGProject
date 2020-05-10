using System;
using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] public float speed = 100f; // vitesse du drone
    
    private Rigidbody2D _rigidbody;
    public Vector2 currentForce;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 endPos)
    {
        _rigidbody.angularVelocity = 0f;
        if (endPos != Vector2.zero)
        {
            currentForce = endPos * speed;
            _rigidbody.AddForce(endPos * speed);
        }
        else
        {
            currentForce = _rigidbody.mass * _rigidbody.gravityScale * Physics2D.gravity;
            _rigidbody.AddForce(currentForce);
        }
    }
}