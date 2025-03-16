using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Events
    public event Action<int> OnEnemyCountChanged;

    public event Action OnWin;
    public event Action OnLose;
    
    #endregion
    
    #region Internal State
    private int _nEnemies = 0;
    public int NEnemies => _nEnemies;
    
    private Player _player;

    public enum GameState 
    {
        Playing,
        Lose, 
        Win,
    }
    private GameState _gameState = GameState.Playing;
    public GameState CurrentGameState => _gameState;

    private InputAction _restartAction;
    
    #endregion

    private void Awake()
    {
        _player = FindFirstObjectByType<Player>();
        RegisterEnemies(FindEnemies());
    }

    void Start()
    {
        // This needs to be on start so that PlayerHealth is set by then.
        // Player health is set in player's awake
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

    void WinGame()
    {
        _gameState = GameState.Win;
        OnWin?.Invoke();
    }

    void LoseGame()
    {
        _player.DeactivateInput();
        _gameState = GameState.Lose;
        OnLose?.Invoke();
    }

    void PlayerHealthChanged(Health health, Health.Change change)
    {
        if (change.JustDied(health))
            LoseGame();
    }

    Enemy[] FindEnemies() => FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

    void OnRestartPressed(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
