using System;
using System.Collections;
using Spline;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int point;
    [SerializeField] private BezierSpline pattern;
    [SerializeField] private float patternDuration = 40;
    [SerializeField] private float followDuration = 200;

    private Vector3 _spawnPos;
    private bool _isMoving;
    private Coroutine _currentCoroutine;

    /**
    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            Vector3 startPos = other.transform.position;

            Vector3 endPos = startPos + (Vector3)bullet.GetComponent<Rigidbody2D>().velocity.normalized;

            Player player = bullet.Origin.GetComponentInParent<Player>();
            Drone drone = bullet.Origin.GetComponentInParent<Drone>();
            if (player != null)
            {
                player.Score += this.Point;
                //Explose();
                Destroy(gameObject);
            }
        }
    }**/

    private void Start()
    {
        _spawnPos = transform.position;
    }

    private void FixedUpdate()
    {
        Debug.Log(_isMoving);
        if (!_isMoving)
            _currentCoroutine = StartCoroutine(FollowPatternCorourtine());
    }

    IEnumerator FollowPatternCorourtine()
    {
        _isMoving = true;

        var elapsedTime = 0f;
        transform.position = pattern.GetPoint(elapsedTime);
        
        while (elapsedTime <= patternDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            var point = pattern.GetPoint(elapsedTime / patternDuration);
            transform.position = point;
            
            // We look at a further point in time (here 5% of moveDuration)
            var nextPoint = pattern.GetPoint(
                ((elapsedTime + (patternDuration * 0.05f)) / patternDuration) % 1);
            LookAtPosition(nextPoint);
            
            yield return null;
        }

        yield return _isMoving = false;
    }
    
    private void LookAtPosition(Vector3 targetPosition)
    {
        Transform transf = transform;
        Vector3 pos = transf.position;

        Vector2 direction = new Vector2(
            targetPosition.x - pos.x,
            targetPosition.y - pos.y
        );

        transf.up = direction;
    }

    private void LookAtTarget(Transform target)
    {
        Transform transf = transform;
        Vector3 pos = transf.position;

        Vector3 targetPosition = target.transform.position;
            
        Vector2 direction = new Vector2(
            targetPosition.x - pos.x,
            targetPosition.y - pos.y
        );

        transf.up = direction;
    }
    
    IEnumerator FollowTargetCoroutine(Vector3 targetPos)
    {
        _isMoving = true;

        var endPos = targetPos;
        var elapsedTime = 0f;

        while (elapsedTime <= followDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(transform.position, endPos, elapsedTime / followDuration);
            
            // We look at a further point in time (here 5% of moveDuration)
            var nextPoint = Vector3.Lerp(transform.position, endPos,
                ((elapsedTime + (followDuration * 0.05f)) / followDuration) % 1);

            LookAtPosition(nextPoint);
            
            yield return null;
        }

        yield return _isMoving = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _isMoving = false;
            }
            
            var playerPos = other.transform.position;
            var shooter = GetComponentInChildren<Shooter>();

            if (shooter != null)
            {
                shooter.Shoot(playerPos);    
            }
            
            _currentCoroutine = 
                StartCoroutine(FollowTargetCoroutine(playerPos));
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            var playerPos = other.transform.position;
            var shooter = GetComponentInChildren<Shooter>();

            if (shooter != null)
            {
                shooter.Shoot(playerPos);    
            }
            
            if (!_isMoving)
            {
                _currentCoroutine = 
                    StartCoroutine(FollowTargetCoroutine(playerPos));
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _isMoving = false;
            }
            
            _currentCoroutine = 
                StartCoroutine(FollowTargetCoroutine(_spawnPos));
        }
    }
}