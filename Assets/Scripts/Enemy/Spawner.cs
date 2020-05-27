using System;
using Spline;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private BezierSpline pattern;
    [SerializeField] private float patternDuration = 20f;
    [SerializeField] private int loopTimes = 1;
    [SerializeField] private float spawnDelay = 2.0f;
    [SerializeField] private int spawnNumber = 2;
    private bool _canSpawn;
    private int currentlySpawned = 0;
    private float elapsedTime;

    private void Update()
    {
        if (_canSpawn)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > spawnDelay && currentlySpawned < spawnNumber)
            {
                elapsedTime = 0f;
                Spawn();
            }
        }
    }

    void Spawn()
    {
        currentlySpawned++;
        var chosenPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        var enemyGo = Instantiate (chosenPrefab,pattern.GetPoint(0),Quaternion.identity,
            LevelsManager.Instance.CurrentLevel.transform);
        var enemy = enemyGo.GetComponent<Enemy>();
        enemy.pattern = pattern;
        enemy.patternDuration = patternDuration;
        enemy.loopTimes = loopTimes;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.GetComponent<Player>())
        {
            _canSpawn = true;
        }
    }
}
