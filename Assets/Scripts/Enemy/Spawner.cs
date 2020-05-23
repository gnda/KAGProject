using Spline;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private BezierSpline pattern;
    [SerializeField] private float spawnDelay = 2.0f;
    [SerializeField] private int spawnNumber = 2;
    private int currentlySpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating ("Spawn", spawnDelay, spawnDelay);
    }
    
    void Spawn()
    {
        if (currentlySpawned < spawnNumber)
        {
            currentlySpawned++;
            var chosenPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            var enemyGo = Instantiate (chosenPrefab,pattern.GetPoint(0),Quaternion.identity);
            enemyGo.GetComponent<Enemy>().pattern = pattern;
        }
        else
        {
            CancelInvoke("Spawn");
        }
    }
}
