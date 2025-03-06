using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Health))]
public class EnemyAnimation : MonoBehaviour
{
    #region Components
    Animator _animator;
    Health _health;
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _health.OnHealthChanged += OnHurt;
    }

    private void OnHurt(Health health, int offset, Vector3 direction)
    {
        if (health.isDead && offset < 0)
        {
            _animator.SetTrigger(EnemyAnimationParams.Die);
            return;
        }
        
        if (offset < 0)
            _animator.SetTrigger(EnemyAnimationParams.Hurt);
    }
}
