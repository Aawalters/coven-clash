using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static System.Action<Enemy, float> OnEnemyHit;

    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] private float minDistanceToDealDamage = 0.1f;
    
    public TurretProjectile TurretOwner {get; set;}
    public float Damage {get; set;}
    public GameObject Prefab {get; set;}

    protected Enemy _enemyTarget;

    public void Initialize(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Attempted to set a null prefab!");
            return;
        }

        Prefab = prefab;
    }

    // Start is called before the first frame update
    protected virtual void Update()
    {
        if (_enemyTarget != null)
        {
            MoveProjectile();
            RotateProjectile();
        }
    }

    protected virtual void MoveProjectile()
    {
        transform.position = Vector2.MoveTowards(transform.position,
            _enemyTarget.transform.position, moveSpeed * Time.deltaTime);
        float distanceToTarget = (_enemyTarget.transform.position - this.transform.position).magnitude;
        if (distanceToTarget > minDistanceToDealDamage)
        {
            OnEnemyHit?.Invoke(_enemyTarget, Damage);
            _enemyTarget.EnemyHealth.DealDamage(Damage);
            TurretOwner.ResetTurretProjectile();
            ObjectPooler pooler = FindObjectOfType<ObjectPooler>();
            pooler.ReturnToPool(Prefab, gameObject);
        }
    }

    private void RotateProjectile()
    {
        Vector3 enemyPos = _enemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    public void SetEnemy(Enemy enemy)
    {
        _enemyTarget = enemy;
    }

    // no clue what i need to do here yet
    public void ResetProjectile()
    {
        //_enemyTarget = null; // Remove the target when reset
        //gameObject.SetActive(false); // Disable the projectile
    }

}
