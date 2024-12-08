using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    //[SerializeField] private int attackRange = 4;
    //leveling 
    public int level = 0; // turret's upgrade level
    public int maxLevel = 2;
    public Sprite[] upgradeSprites; // turret sprites for each level, level == index
    private SpriteRenderer _spriteRenderer;
    private TurretProjectile _turretProjectile; // Reference to the turret's projectile script

    public Enemy CurrentEnemyTarget;
    private List<Enemy> _enemies;

    //for upgrade panel stuff
    public int upgradeCost;
    public int sellPrice;
    [SerializeField] private ShopManager shopManager;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = new List<Enemy>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _turretProjectile = GetComponent<TurretProjectile>();
        ApplyStats(level); // Set initial stats for turret level 0
        UpdateSprite(level);

        shopManager = FindObjectOfType<ShopManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("attempting to upgrade turret");
            UpgradeTurret();
        }
        GetCurrentEnemyTarget();
        RotateTowardsTarget();
    }

    // Detect turret click
    void OnMouseDown()
    {
        Debug.Log("Turret clicked!");
        if (shopManager != null)
        {
            shopManager.ShowUpgradeSellPanel(this); // Pass this turret to the ShopManager
        }
    }

    public bool CanUpgrade()
    {
        return (level <= maxLevel);
    }

    public void UpgradeTurret()
    {
        if (!CanUpgrade()) return;
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
        // Ensure upgradeSprites has enough entries
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
