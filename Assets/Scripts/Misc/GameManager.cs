using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Internal State

    private int _nEnemies = 0;
    private Player _player;
    
    #endregion

    private void Awake()
    {
        _player = FindFirstObjectByType<Player>();
    }

    void OnEnable()
    {
        RegisterEnemies(FindEnemies());
        _player.Health.OnHealthChanged += PlayerHealthChanged;
    }

    void OnDisable()
    {
        DeRegisterEnemies(FindEnemies());
    }

    void RegisterEnemies(Enemy[] enemies)
    {
        foreach (var enemy in enemies)
        {
            _nEnemies++;
            enemy.OnJustDied += OnEnemyKilled;
        }
    }

    void DeRegisterEnemies(Enemy[] enemies)
    {
        foreach (var enemy in enemies)
        {
            enemy.OnJustDied -= OnEnemyKilled;
        }
    }

    void OnEnemyKilled()
    {
        _nEnemies--;
        if (_nEnemies == 0)
            WinGame();
    }

    void WinGame()
    {
                
    }

    void PlayerHealthChanged(Health health, Health.Change change)
    {
        
    }

    Enemy[] FindEnemies() => FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
}
