using System;
using UnityEngine;

public class KillAllEnemiesGameManager : GameManager
{
    #region Events
    public event Action<int> OnEnemyCountChanged;
    #endregion
    
    #region Internal State
    private int _nEnemies = 0;
    public int NEnemies => _nEnemies;
    #endregion

    void Start()
    {
        // Register enemies on start so that everything that depends on this manager
        // can wire itself in case they are interested in this
        RegisterEnemies(FindEnemies());
        // This needs to be on start so that PlayerHealth is set by then.
        // Player health is set in player's awake
        if (_player)
            _player.Health.OnHealthChanged += PlayerHealthChanged;
        
        // Set up input for restart
        _restartAction.performed += OnRestartPressed;
    }

    void OnDisable()
    {
        DeRegisterEnemies(FindEnemies());
    }

    void RegisterEnemies(Enemy[] enemies)
    {
        foreach (var enemy in enemies)
        {
            SetNEnemies(NEnemies + 1);
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
        SetNEnemies(NEnemies-1);
        if (NEnemies == 0)
            WinGame();
    }

    void SetNEnemies(int newVal)
    {
        _nEnemies = newVal;
        OnEnemyCountChanged?.Invoke(_nEnemies);
    }

    protected override void WinGame()
    {
        _gameState = GameState.Win;
        CallWin();
    }

    protected override void LoseGame()
    {
        _player.DeactivateInput();
        _gameState = GameState.Lose;
        CallLose();
    }

    Enemy[] FindEnemies() => FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
}
