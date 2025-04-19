using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ProcKillEmAllGameMode : GameManager
{
    #region Inspector Properties
    
    [SerializeField]
    private RoomGen roomGen;
    
    [SerializeField]
    private PlayerAmmoGUI _ammoGUI;

    [SerializeField] private PlayerHealthGUI _healthGUI;

    [SerializeField] private TextMeshProUGUI enemiesToKill;
    
    #endregion
    
    #region Internal State

    private int _nEnemies;
    private PlayerStart _playerStart;
    
    #endregion
    
    private void Reset()
    {
        roomGen = FindFirstObjectByType<RoomGen>(FindObjectsInactive.Include);
        _ammoGUI = FindFirstObjectByType<PlayerAmmoGUI>();
        _healthGUI = FindFirstObjectByType<PlayerHealthGUI>(); 
    }
    
    private void Start()
    {
        _player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
        
        _restartAction.performed += OnRestartPressed;
        roomGen.GenerateRooms();
    }

    private void OnEnable()
    {
        roomGen.OnGenerationFinished += StartOnGenFinished;
    }

    private void OnDisable()
    {
        roomGen.OnGenerationFinished -= StartOnGenFinished;
    }

    private void StartOnGenFinished()
    {
        StartCoroutine(DelayOnGenerationFinished());
    }

    private IEnumerator DelayOnGenerationFinished()
    {
        yield return null;
        OnGenerationFinished();
    }

    private void OnGenerationFinished()
    {
        RegisterAllEnemies();
        FindPlayerStart();
    }

    private void FindPlayerStart()
    {
        _playerStart = FindFirstObjectByType<PlayerStart>();
        if (_playerStart)
            _playerStart.SpawnPlayer();
        
        // Set player stuff here since in start is not initialized
        if (_player)
            _player.Health.OnHealthChanged += PlayerHealthChanged;
        _ammoGUI.SetPlayer(_player);
        _healthGUI.SetPlayer(_player);
    }

    private void RegisterAllEnemies()
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        _nEnemies = enemies.Length;

        foreach (var enemy in enemies)
            enemy.OnJustDied += OnEnemyDied;
        
        UpdateEnemiesToKill();
    }

    protected override void LoseGame()
    {
        Debug.Log("<color=red>You Lose</color>");
    }

    protected override void WinGame()
    {
        Debug.Log("<color=green>You Win</color>");
    }

    private void OnEnemyDied()
    {
        _nEnemies = Mathf.Max(_nEnemies - 1, 0);
        UpdateEnemiesToKill();
        if (_nEnemies == 0) 
            WinGame();
    }

    private void UpdateEnemiesToKill()
    {
        if (enemiesToKill)
            enemiesToKill.text = _nEnemies.ToString();
    }
}
