using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    private Animator _animator;
    private Enemy _enemy;
    private EnemyHealth _enemyHealth;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    private void PlayHurtAnimation()
    {
        //_animator.SetTrigger("Hurt");
    }

    private void PlayDeathAnimation()
    {
       // _animator.SetTrigger("Death");
    }

    private float GetCurrentAnimationLength()
    {
        //float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        //return animationLength;
        return 0.5f;
    }

    private IEnumerator PlayHurt()
    {
        _enemy.StopMovement();
        PlayHurtAnimation();
        //Debug.Log("enemy hit! stopping movement.");
        yield return new WaitForSeconds(GetCurrentAnimationLength() + 0.3f);
        //Debug.Log("ok it's ok now, resuming movement.");
        _enemy.ResumeMovement();
    }

    private IEnumerator PlayDead()
    {
        _enemy.StopMovement();
        PlayDeathAnimation();
        yield return new WaitForSeconds(GetCurrentAnimationLength() + 0.3f);
        _enemy.ResumeMovement();
        _enemyHealth.ResetHealth();
        
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

    private void EnemyHit(Enemy enemy)
    {
        if (_enemy == enemy)
        {
            StartCoroutine(PlayHurt());
        }
    }

    private void EnemyDead(Enemy enemy)
    {
        if (_enemy == enemy)
        {
            StartCoroutine(PlayDead());
        }
    }

    private void OnEnable() 
    {
        EnemyHealth.OnEnemyHit += EnemyHit;
        EnemyHealth.OnEnemyKilled += EnemyDead;
    }

    private void OnDisable() 
    {
        EnemyHealth.OnEnemyHit -= EnemyHit;
        EnemyHealth.OnEnemyKilled -= EnemyDead;
    }
}
