using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    //[SerializeField] private int attackRange = 4;
    //leveling 
    public int level = 0;                   // turret's upgrade level
    public int maxLevel = 2;                // turret's max upgrade level
    public Sprite[] upgradeSprites;         // turret sprites for each level, level == index
    [SerializeField] public SpriteRenderer _spriteRenderer;
    private TurretProjectile _turretProjectile; // ref to the turret's projectile script

    public Enemy CurrentEnemyTarget;        // enemy that is furthest along & being targeted
    private List<Enemy> _enemies;           // list of all enemies within turret range

    //for upgrade panel stuff
    public int purchaseCost;
    public int upgradeCost;
    public int sellPrice;
    [SerializeField] private ShopManager shopManager;

    void Start()
    {
        _enemies = new List<Enemy>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        _turretProjectile = GetComponent<TurretProjectile>();
        ApplyStats(level); // set initial stats for turret level 0
        UpdateSprite(level);

        shopManager = FindObjectOfType<ShopManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("attempting to upgrade turret");
            UpgradeTurret();
        }
        GetCurrentEnemyTarget();
        //RotateTowardsTarget();
    }

    // detect turret click for the upgrade/sell panel to appear
    /*
    void OnMouseDown()
    {
        Debug.Log("Turret clicked!");
        if (shopManager != null)
        {
            //shopManager.ShowUpgradeSellPanel(this); // pass this turret ref to the ShopManager
        }
    }
    */

    public bool CanUpgrade()
    {
        return (level <= maxLevel);
    }

    public void UpgradeTurret()
    {
        if (!CanUpgrade()) return; // no cheating the system allowed
        level++;
        ApplyStats(level);
        UpdateSprite(level);
    }

    private void ApplyStats(int upgradeLevel)
    {
        // modify stats based on the upgrade level
        switch (upgradeLevel)
        {
            case 0:
                _turretProjectile.Damage = 5f;
                _turretProjectile.DelayPerShot = 0.8f;
                break;
            case 1:
                _turretProjectile.Damage = 10f;
                _turretProjectile.DelayPerShot = 1f;
                break;
            case 2:
                _turretProjectile.Damage = 20f;
                _turretProjectile.DelayPerShot = 1.2f;
                break;
            default:
                _turretProjectile.Damage = 5f;
                _turretProjectile.DelayPerShot = 0.8f;
                break;
        }
    }

    private void UpdateSprite(int upgradeLevel)
    {
        // ensure upgradeSprites has enough entries
        if (upgradeLevel < upgradeSprites.Length)
        {
            _spriteRenderer.sprite = upgradeSprites[upgradeLevel];
        }
    }
    
    //enemy targeting 
    private void GetCurrentEnemyTarget() 
    {
        if (_enemies.Count <= 0)
        {
            CurrentEnemyTarget = null;
            return;
        }
        // sort enemies by progress (so the one further along is prioritized)
        _enemies.Sort((enemyA, enemyB) => Enemy.CompareProgress(enemyA, enemyB));

        // remove any dead or invalid enemies from the list
        _enemies.RemoveAll(enemy => enemy == null || !enemy.isAlive); 

        // after cleanup, pick the new target if it exists
        CurrentEnemyTarget = _enemies.Count > 0 ? _enemies[0] : null;
    }

    private void RotateTowardsTarget() 
    {
        if (CurrentEnemyTarget == null) return;

        Vector3 targetPos = CurrentEnemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, targetPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    //both for detecting when enemy in range, stored in _enemies
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
            //if (_enemies.Contains(enemy))
            //{
                _enemies.Remove(enemy);
            //}
        }
    }
}
