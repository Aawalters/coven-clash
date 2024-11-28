using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    [SerializeField] protected Transform projectileSpawnPostion;
    [SerializeField] protected float delayBtwnAttacks = 2f;
    [SerializeField] protected float damage = 2f;
    [SerializeField] public GameObject prefab; //projectile prefab

    public float Damage {get; set;}
    public float DelayPerShot {get; set;}
    protected float _nextAttackTime;
    protected ObjectPooler _pooler;
    protected Turret _turret;
    protected Projectile _currentProjectileLoaded;

    // Start is called before the first frame update
    private void Start()
    {
        _turret = GetComponent<Turret>();
        _pooler = GetComponent<ObjectPooler>();
        Damage = damage;
        DelayPerShot = delayBtwnAttacks;
        LoadProjectile();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (IsTurretEmpty())
        {
            LoadProjectile();
        }
        if(Time.time > _nextAttackTime)
        {
            if (_turret.CurrentEnemyTarget != null &&
                _currentProjectileLoaded != null &&
                _turret.CurrentEnemyTarget.isAlive)
            {
                _currentProjectileLoaded.transform.parent = null;
                _currentProjectileLoaded.SetEnemy(_turret.CurrentEnemyTarget);
            }
            _nextAttackTime = Time.time + DelayPerShot;
        }
    }

    protected virtual void LoadProjectile()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool(prefab);
        newInstance.transform.localPosition = projectileSpawnPostion.position;
        newInstance.transform.SetParent(projectileSpawnPostion);

        _currentProjectileLoaded = newInstance.GetComponent<Projectile>();
        _currentProjectileLoaded.TurretOwner = this;
        _currentProjectileLoaded.ResetProjectile();  //does nothing rn
        _currentProjectileLoaded.Damage = Damage;
        newInstance.SetActive(true);
    }

    private bool IsTurretEmpty()
    {
        return _currentProjectileLoaded == null;
    }
    
    public void ResetTurretProjectile()
    {
        _currentProjectileLoaded = null;
    }
}
