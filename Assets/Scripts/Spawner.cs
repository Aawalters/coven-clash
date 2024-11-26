using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyType
{
    public GameObject prefab;
    public int count;
}

[System.Serializable]
public class WaveConfigeration
{
    public List<EnemyType> enemyTypes;
}

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private GameObject testGO;
    [SerializeField] private Waypoint defaultWaypoint; // Default waypoint for enemies

    [Header("Fixed Delay")]
    [SerializeField] private float delayBtwnSpawns;

    private float _spawnTimer;
    private int _enemiesSpawned;
    private ObjectPooler _pooler;

    

    // Start is called before the first frame update
    void Start()
    {
        _pooler = GetComponent<ObjectPooler>();
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0)
        {
            _spawnTimer = delayBtwnSpawns;
            if (_enemiesSpawned < enemyCount)
            {
                _enemiesSpawned++;
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();
        

    if (newInstance != null)
    {
        Waypoint waypoint = GetComponent<Waypoint>();
        if (waypoint == null || waypoint.Points.Length == 0)
        {
            Debug.LogError("Waypoint system is not properly set up on the Spawner!");
            return;
        }

        Vector3 firstWaypointPosition = waypoint.GetWaypointPos(0);

        Enemy enemyComponent = newInstance.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            Debug.Log($"Spawning enemy at {firstWaypointPosition}");
            enemyComponent.ResetEnemy(firstWaypointPosition, waypoint);
        }
        else
        {
            Debug.LogError("Enemy component not found on spawned instance!");
        }

        newInstance.SetActive(true);
    }
    }
}
