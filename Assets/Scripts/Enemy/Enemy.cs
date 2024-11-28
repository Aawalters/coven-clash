using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private int DeathCoinReward;
    [SerializeField] private Waypoint Waypoint;

    private SpriteRenderer _spriteRenderer;
    private Vector3 _lastPointPosition;
    private int _currentWaypointIndex = 0;
    private EnemyHealth _enemyHealth;
    private bool moveEnabled;

    public GameObject Prefab { get; set; } // Reference to the prefab for this enemy
    public static event System.Action<Enemy> OnEndReached;
    public bool isAlive;

    // Properties to track progress
    public int CurrentWaypointIndex => _currentWaypointIndex; 
    public float DistanceToNextWaypoint => (transform.position - CurrentPointPosition).magnitude;

    public void Initialize(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Attempted to set a null prefab!");
            return;
        }

        Prefab = prefab;
    }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _lastPointPosition = transform.position;
        moveEnabled = true;
        isAlive = true;
    }

    void Update()
    {
        Move();
        Rotate();

        if (CurrentPointPositionReached())
        {
            UpdateCurrentPointIndex();
        }
    }

    private void Move()
    {
        if (moveEnabled)
        {
            transform.position = Vector3.MoveTowards(
            transform.position,
            CurrentPointPosition,
            MoveSpeed * Time.deltaTime); 
        }
    }

    private void Rotate()
    {
        if (CurrentPointPosition.x > _lastPointPosition.x)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }

    private bool CurrentPointPositionReached()
    {
        float distanceToNextPointPosition = (transform.position - CurrentPointPosition).magnitude;
        if (distanceToNextPointPosition < 0.1f)
        {
            _lastPointPosition = transform.position;
            return true;
        }
        return false;
    }

    private void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = Waypoint.Points.Length - 1;
        if (_currentWaypointIndex < lastWaypointIndex)
        {
            _currentWaypointIndex++;
        }
        else
        {
            EndPointReached();
        }
    }

    private void EndPointReached()
    {
        OnEndReached?.Invoke(this);
        _enemyHealth.ResetHealth();

        if (Prefab != null)
        {
            ObjectPooler pooler = FindObjectOfType<ObjectPooler>();
            isAlive = false;
            pooler.ReturnToPool(Prefab, gameObject);
        }
        else
        {
            Debug.LogWarning("Prefab reference is missing on the enemy when returning to the pool.");
        }
    }

    private Vector3 CurrentPointPosition => Waypoint.Points[_currentWaypointIndex];

    public void ResetEnemy(Vector3 position, Waypoint waypoint)
    {
        if (waypoint == null)
        {
            Debug.LogError("Waypoint passed to ResetEnemy is null!");
            return;
        }

        transform.position = position; // Reset position to the first waypoint
        Waypoint = waypoint;           // Reassign the waypoint system
        _currentWaypointIndex = 0;     // Reset the waypoint index
        _enemyHealth.ResetHealth();    // Reset health if necessary
        isAlive = true;
    }

    public void StopMovement()
    {
        moveEnabled = false;
    }

    public void ResumeMovement()
    {
        moveEnabled = true;
    }

    // Static method for comparing progress of two enemies
    public static int CompareProgress(Enemy enemyA, Enemy enemyB)
    {
        Debug.Log($"comparing two enemies. enemy A index {enemyA.CurrentWaypointIndex}, enemy B index {enemyB.CurrentWaypointIndex}");
        // First compare based on waypoint index (further along in the path is prioritized)
        if (enemyA.CurrentWaypointIndex < enemyB.CurrentWaypointIndex)
        {
            return 1; // enemyB is further along
        }
        else if (enemyA.CurrentWaypointIndex > enemyB.CurrentWaypointIndex)
        {
            return -1; // enemyA is further along
        }
        else
        {
            // If they are at the same waypoint, prioritize by distance to the next waypoint
            return enemyA.DistanceToNextWaypoint > enemyB.DistanceToNextWaypoint ? 1 : -1;
        }
    }
}
