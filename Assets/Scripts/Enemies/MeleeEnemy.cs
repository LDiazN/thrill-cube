using System;
using UnityEngine;

[RequireComponent(typeof(PunchAttack), typeof(Health))]
public class MeleeEnemy : MonoBehaviour
{
    #region Components

    PunchAttack _punchAttack;
    Health _health;
    
    #endregion

    private void Awake()
    {
        _punchAttack = GetComponent<PunchAttack>();
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += OnDie;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= OnDie;
    }

    private void OnDie(Health health, Health.Change change)
    {
        if (!change.JustDied(health))
            return;
        
        _punchAttack.WantsToPunch = false;
    }
}
