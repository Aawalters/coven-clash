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

    void Start()
    {
        // _spriteRenderer = GetComponent<SpriteRenderer>();
        // _enemyHealth = GetComponent<EnemyHealth>();
        // _lastPointPosition = transform.position;
    }

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
        //Debug.Log($"Resetting enemy to position {position} with waypoint {waypoint.name}");
        _currentWaypointIndex = 0;     // Reset the waypoint index
        _enemyHealth.ResetHealth();    // Reset health if necessary
    }

    public void StopMovement()
    {
        moveEnabled = false;
    }

    public void ResumeMovement()
    {
        moveEnabled = true;
    }

}
