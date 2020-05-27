using System;
using System.Collections;
using Spline;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int points;
    public BezierSpline pattern;
    public float patternDuration = 10;
    public int loopTimes = 1;
    [SerializeField] private float goingBackDuration = 20;
    [SerializeField] private float followSpeed = 20;
    [SerializeField] private float distanceFromTarget = 0;
    [SerializeField] private bool canMove = true;
    
    private Vector3 _lastPos, _targetPos;
    private Quaternion _spawnRot;
    private bool _isFollowingTarget, _isFollowingPattern, _isGoingBack;

    private Rotatable _rotatable;
    private Shooter[] _shooters;
    private float patternTimer, goingBackTimer;
    private int currentLoopTimes;

    private void Awake()
    {
        foreach (var e in GameManager.Instance.Enemies)
        {
            foreach (var col in e.GetComponents<Collider2D>())
            {
                Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
            }
            
            foreach (var col in e.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
            }
        }
    }

    private void Start()
    {
        _rotatable = GetComponentInChildren<Rotatable>();
        _shooters = GetComponentsInChildren<Shooter>();
        
        if (pattern != null)
        {
            _isFollowingPattern = true;
        }

        _spawnRot = transform.rotation;
    }

    private void Update()
    {
        if (currentLoopTimes >= loopTimes)
        {
            Destroy(gameObject);
        }
        if (_isFollowingPattern && canMove)
        {
            FollowPattern();
        }
        else if (_isGoingBack && canMove)
        {
            GoBack();
        } 
        else if (_isFollowingTarget)
        {
            FollowTarget();
        }
    }

    private void FollowPattern()
    {
        patternTimer += Time.deltaTime;
        if (patternTimer > patternDuration)
        {
            patternTimer = 0;
            transform.position = pattern.GetPoint(0);
            transform.rotation = _spawnRot;
            currentLoopTimes++;
        }
        else
        {
            float timePerc = patternTimer / patternDuration;
            var point = pattern.GetPoint(timePerc);
            transform.position = point;
                
            var nextPoint = pattern.GetPoint(
                ((patternTimer + (patternDuration * 0.05f)) / patternDuration) % 1);

            if (_rotatable)
            {
                _rotatable.Rotate(nextPoint);
            }
        }
    }

    private void GoBack()
    {
        float patternTimePerc = patternTimer / patternDuration;
        var point = pattern.GetPoint(patternTimePerc);
        
        goingBackTimer += Time.deltaTime;
        if (goingBackTimer > goingBackDuration)
        {
            goingBackTimer = 0;
            transform.position = point;
            transform.rotation = _spawnRot;
            _isGoingBack = false;
            _isFollowingPattern = true;
        }
        else
        {
            float timePerc = goingBackTimer / goingBackDuration;
            
            transform.position = Vector3.Lerp(_lastPos, point, timePerc);
            
            if (_rotatable)
            {
                _rotatable.Rotate(point);
            }
        }
    }
    
    private void FollowTarget()
    {
        if (_shooters != null)
        {
            foreach (var s in _shooters)
            {
                s.LookAt(_targetPos);
                s.Shoot(_targetPos);  
            }
        }

        if (_rotatable)
        {
            _rotatable.Rotate(_targetPos);
        }

        if (canMove)
        {
            if (Vector3.Distance(transform.position, _targetPos) > distanceFromTarget)
            {
                transform.position = 
                    Vector2.MoveTowards(transform.position, _targetPos, 
                        followSpeed * Time.deltaTime);                    
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            _isFollowingPattern = false;
            _isGoingBack = false;
            goingBackTimer = 0;
            
            _targetPos = other.transform.position;
            _isFollowingTarget = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            _lastPos = transform.position;
            
            if (canMove && pattern != null)
            {
                _isGoingBack = true;
            }
            _isFollowingTarget = false;
        }
    }
}