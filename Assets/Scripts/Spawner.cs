using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [Header("-------- Settings --------")]
    [SerializeField] private List<WaveConfigeration> waves;
    [SerializeField] private Waypoint defaultWaypoint; 
    [SerializeField] private Button waveControlButton;
    [SerializeField] private TMP_Text buttonText; 

    private int _currentWaveIndex = 0;
    private float _spawnTimer;
    public ObjectPooler _pooler;
    private Queue<EnemyType> _currentWaveQueue = new Queue<EnemyType>();
    private bool _isSpawning = false;
    private bool _isPaused = false;
    public int _activeEnemies;
    AudioManager audioManager;

    void Start()
    {
        _pooler = GetComponent<ObjectPooler>();
        Debug.Log($" wave count is {waves.Count}");
        audioManager = AudioManager.Instance;
        // Setup button behavior
        waveControlButton.onClick.AddListener(ToggleWaveControl);
        buttonText.text = "Start";
        Time.timeScale = 1; 
    }

    void Update()
    {
        if (_currentWaveIndex >= waves.Count && _activeEnemies == 0)
        {
            // victory!! 
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            levelManager.Win();
        }
        if (_isSpawning && _currentWaveQueue.Count == 0 && _activeEnemies == 0)
        {
            EndCurrentWave();
            return;
        }
        if (!_isSpawning || _isPaused || _currentWaveQueue.Count == 0) return;

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
        _activeEnemies = 0;

        // populate the queue for this wave
        foreach (EnemyType enemyType in waves[_currentWaveIndex].enemyTypes)
        {
            if (enemyType.prefab == null)
            {
                Debug.LogError("Enemy prefab is not assigned!");
                return;
            }
            for (int i = 0; i < enemyType.count; i++)
            {
                _currentWaveQueue.Enqueue(enemyType);
            }
        }

        _spawnTimer = 0;
        _isSpawning = true;
        _isPaused = false;
        buttonText.text = "Pause";
    }

    private void EndCurrentWave()
    {
        _isSpawning = false;
        _currentWaveIndex++;
        buttonText.text = "Start";
       TriggerWaveDialogue();
    }

    private void TriggerWaveDialogue()
    {
        if (_currentWaveIndex < waves.Count)
        {
            string knotName = $"Wave{_currentWaveIndex}"; 
            DialogueManager.Instance.StartDialogueAt(knotName);
        }
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
                //Debug.Log($"Spawning enemy at {firstWaypointPosition}");
                //Debug.Log($"Spawning enemy of type: {enemyTypeToSpawn.prefab.name}");
                enemyComponent.ResetEnemy(firstWaypointPosition, waypoint);
                Debug.Assert (enemyTypeToSpawn.prefab != null, "bro why is the prefab for the enemy to spawn null");
                enemyComponent.Prefab = enemyTypeToSpawn.prefab;
                //Debug.Log($"enemy should have prefab of type {enemyTypeToSpawn.prefab}");
                //Debug.Log($"actual: {enemyComponent.Prefab}");
            }
            else
            {
                Debug.LogError("Enemy component not found on spawned instance!");
            }

            _activeEnemies++; 
            newInstance.SetActive(true);
        }
    }

    private void ToggleWaveControl()
    {
        audioManager.PlaySFX(audioManager.click);
        if (!_isSpawning) // Start a new wave
        {
            StartNextWave();
        }
        else if (_isPaused) // Resume the game
        {
            _isPaused = false;
            Time.timeScale = 1; // Resume game time
            buttonText.text = "Pause";
        }
        else // Pause the game
        {
            _isPaused = true;
            Time.timeScale = 0; // Freeze game time
            buttonText.text = "Play";
        }
    }
}
