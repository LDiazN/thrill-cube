using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class AIGuide : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] private float timeBetweenEnemyUpdates = 1;
    [SerializeField] private RangeDetector walkingDistance;
    [SerializeField] private RangeDetector shootingDistance;
    public RangeDetector ShootingDistance => shootingDistance;

    #endregion
    
    #region Internal State

    private List<Enemy> _enemies = new();
    private float _timeSinceLastEnemyUpdate;
    [CanBeNull] private Enemy _closestEnemy;
    private bool _shouldShoot;

    public bool ShouldShoot
    {
        get => _shouldShoot;
        set => _shouldShoot = value;
    }

    public Player player;

    #endregion
    
    #region Components
    private NavMeshAgent _agent;
    #endregion

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateEnemies();
        UpdateOrientation();
    }

    private void UpdateOrientation()
    {
        if (_agent.velocity.sqrMagnitude > 0.1f)
            transform.LookAt(transform.position + _agent.velocity.normalized);
    }

    private void UpdateClosestEnemy()
    {
        var minDistance = Mathf.Infinity;
        Enemy closestEnemy = null;
        foreach (var enemy in _enemies)
        {
            var path = new NavMeshPath();
            var position = enemy.transform.position;
            // Make sure is in the same level as the player to ensure the path is valid 
            // Some enemies might have the pivot point in a higher position
            position.y = transform.position.y;
            if (_agent.CalculatePath(position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                // Compute distance by navigation, not just euclidian distance
                float distance = 0;
                for (int i = 1; i < path.corners.Length; i++)
                    distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                
                if (distance < minDistance)
                {
                    closestEnemy = enemy;
                    minDistance = distance;
                }
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
            if (!enemy.Health.isDead)
                _enemies.Add(enemy);
        
        UpdateClosestEnemy();
    }
    
    public Enemy GetClosestEnemy()
    {
        // Looks stupid but actually unity is checking if the object is not only null, 
        // but also a valid object in scene
        return _closestEnemy != null ? _closestEnemy : null;
    }

    public bool EnemyOnSight()
    {
        if (!player)
            return false;

        RaycastHit hit;
        var hitSomething = Physics.Raycast(player.TPSCamera.GetCameraPosition(), player.TPSCamera.GetCameraDirection(), out hit);
        if (!hitSomething)
            return false;

        var enemy = hit.collider.GetComponent<Enemy>();
        if (!enemy)
            return false;

        return !enemy.Health.isDead;
    }
}
