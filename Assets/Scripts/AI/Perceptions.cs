using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Perceptions : MonoBehaviour
{
    #region Components
    Health _health;
    #endregion
    
    #region Perceptions

    /// <summary>
    /// Last game object who hurted you
    /// Can be null
    /// </summary>
    private GameObject _lastHurted;
    public GameObject LastHurted => _lastHurted;
    
    #endregion
    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += OnHurt;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= OnHurt;
    }


    private void OnHurt(Health health, Health.Change change)
    {
        if (!change.IsDamage)
            return;

        _lastHurted = change.perpetrator;
    }
}
