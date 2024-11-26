using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{

    public static System.Action<Enemy> OnEnemyKilled;
    public static System.Action<Enemy> OnEnemyHit;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform barPosition;
    [SerializeField] private float initialHealth = 10f;
    [SerializeField] private float maxHealth = 10;

    public float CurrentHealth {get; set;}

    [SerializeField] private Image _healthBar;
    private Enemy _enemy;

    // Start is called before the first frame update
    void Start()
    {
        CreateHealthBar();
        CurrentHealth = initialHealth;

        _enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DealDamage(5f);
        }
        _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount, 
            CurrentHealth / maxHealth, Time.deltaTime * 10f);
    }

    private void CreateHealthBar()
    {
        if (_healthBar != null) return; // Prevent duplicate health bars
        Debug.Log($"Creating health bar for {gameObject.name}");
        
        GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity);
        newBar.transform.SetParent(transform);
        EnemyHealthContainer container = newBar.GetComponent<EnemyHealthContainer>();
        _healthBar = container.FillAmountImage;
    }

    private void DealDamage(float daamgeReceived)
    {
        CurrentHealth -= daamgeReceived;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        else
        {
            OnEnemyHit?.Invoke(_enemy);
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        _healthBar.fillAmount = 1f; // Reset the health bar to full
    }

    private void Die()
    {
        OnEnemyKilled?.Invoke(_enemy); // Notify listeners that the enemy has died
        ResetHealth();

        //return to pool
        if (_enemy.Prefab != null)
        {
            ObjectPooler pooler = FindObjectOfType<ObjectPooler>();
            pooler.ReturnToPool(_enemy.Prefab, _enemy.gameObject);
        }
        else
        {
            Debug.LogWarning("Prefab reference is missing on the enemy. Destroying instead.");
            Destroy(_enemy.gameObject);
        }
    }
}
