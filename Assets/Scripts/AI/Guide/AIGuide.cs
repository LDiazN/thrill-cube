using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AIGuide : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] private float timeBetweenEnemyUpdates = 1;
    
    #endregion
    #region Internal State

    private List<Enemy> _enemies = new();
    private float _timeSinceLastEnemyUpdate;
    [CanBeNull] private Enemy _closestEnemy;
    
    #endregion
    
    #region Components
    private RangeDetector _rangeDetector;
    public RangeDetector RangeDetector => _rangeDetector;
    #endregion

    private void Awake()
    {
        _rangeDetector = GetComponent<RangeDetector>();
    }

    private void Update()
    {
        UpdateEnemies();
    }

    private void UpdateClosestEnemy()
    {
        var minDistance = Mathf.Infinity;
        Enemy closestEnemy = null;
        foreach (var enemy in _enemies)
        {
            var toEnemy = enemy.transform.position - transform.position;
            var distance = toEnemy.magnitude;
            if (distance < minDistance)
            {
                closestEnemy = enemy;
                minDistance = distance;
            }
        }

        _closestEnemy = closestEnemy;
    }

    private void OnDrawGizmos()
    {
        var enemy = GetClosestEnemy();
        if (!enemy)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(enemy.transform.position, 1);
    }

    private void UpdateEnemies()
    {
        _timeSinceLastEnemyUpdate += Time.deltaTime;
        if (_timeSinceLastEnemyUpdate < timeBetweenEnemyUpdates)
            return;

        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        _enemies.Clear();
        foreach (var enemy in enemies)
            _enemies.Add(enemy);
        
        UpdateClosestEnemy();
    }
    
    public Enemy GetClosestEnemy()
    {
        // Looks stupid but actually unity is checking if the object is not only null, 
        // but also a valid object in scene
        return _closestEnemy != null ? _closestEnemy : null;
    }
}
