using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private int attackRange = 4;
    private Enemy CurrentEnemyTarget;
    private List<Enemy> _enemies;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = new List<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentEnemyTarget();
        RotateTowardsTarget();
    }

    private void GetCurrentEnemyTarget() 
    {
        if (_enemies.Count <= 0)
        {
            CurrentEnemyTarget = null;
            return;
        }
        // Sort enemies by progress (so the one further along is prioritized)
        _enemies.Sort((enemyA, enemyB) => Enemy.CompareProgress(enemyA, enemyB));

        // Remove any dead or invalid enemies from the list
        _enemies.RemoveAll(enemy => enemy == null || !enemy.isAlive); 

        // After cleanup, pick the new target (if any)
        CurrentEnemyTarget = _enemies.Count > 0 ? _enemies[0] : null;
    }

    private void RotateTowardsTarget() 
    {
        if (CurrentEnemyTarget == null) return;

        Vector3 targetPos = CurrentEnemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, targetPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy newEnemy = other.GetComponent<Enemy>();
            _enemies.Add(newEnemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        //Debug.Log("trigger exit 2d proc'ing");
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (_enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
            }
        }
    }
}
