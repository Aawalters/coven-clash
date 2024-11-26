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
    public float delayBtwnSpawns;
}

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<WaveConfigeration> waves;
    [SerializeField] private Waypoint defaultWaypoint; // Default waypoint for enemies

    private int _currentWaveIndex = 0;
    private float _spawnTimer;
    private ObjectPooler _pooler;
    private Queue<EnemyType> _currentWaveQueue = new Queue<EnemyType>();
    private bool _isSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        _pooler = GetComponent<ObjectPooler>();
        Debug.Log($" wave count is {waves.Count}");
        StartNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isSpawning || _currentWaveQueue.Count == 0) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            _spawnTimer = waves[_currentWaveIndex].delayBtwnSpawns;
            SpawnEnemy();
        }
    }

    private void StartNextWave()
    {
        if (_currentWaveIndex >= waves.Count)
        {
            Debug.Log("all waves complete");
            _isSpawning = false;
            return;
        }

        Debug.Log($"Starting wave {_currentWaveIndex + 1}");
        _currentWaveQueue.Clear();

        // Populate the queue for this wave
        foreach (EnemyType enemyType in waves[_currentWaveIndex].enemyTypes)
        {
            for (int i = 0; i < enemyType.count; i++)
            {
                _currentWaveQueue.Enqueue(enemyType);
            }
        }

        _spawnTimer = 0;
        _isSpawning = true;
    }

    private void EndCurrentWave()
    {
        _isSpawning = false;
        _currentWaveIndex++;
        Invoke(nameof(StartNextWave), 3f); // Delay before starting the next wave
    }

    private void SpawnEnemy()
    {
        if (_currentWaveQueue.Count == 0)
        {
            EndCurrentWave();
            return;
        }

        EnemyType enemyTypeToSpawn = _currentWaveQueue.Dequeue();

        GameObject newInstance = _pooler.GetInstanceFromPool(enemyTypeToSpawn.prefab);

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
                Debug.Log($"Spawning enemy of type: {enemyTypeToSpawn.prefab.name}");
                enemyComponent.ResetEnemy(firstWaypointPosition, waypoint);
                enemyComponent.Initialize(enemyTypeToSpawn.prefab);
            }
            else
            {
                Debug.LogError("Enemy component not found on spawned instance!");
            }

            newInstance.SetActive(true);
        }
    }
}
