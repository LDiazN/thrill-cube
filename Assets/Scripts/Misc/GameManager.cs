using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public abstract class GameManager : MonoBehaviour
{
    #region Events
    public event Action OnWin;
    public event Action OnLose;
    #endregion
    
    #region Internal State
    protected Player _player;
    
    public enum GameState 
    {
        Playing,
        Lose, 
        Win,
    }
    
    protected GameState _gameState = GameState.Playing;
    public GameState CurrentGameState => _gameState;
    
    protected InputAction _restartAction;
    #endregion

    private void Awake()
    {
        _player = FindFirstObjectByType<Player>();
        _restartAction = InputSystem.actions.FindAction("Restart");
    }

    protected void CallWin() => OnWin?.Invoke();
    protected void CallLose() => OnLose?.Invoke();
    
    
    protected void PlayerHealthChanged(Health health, Health.Change change)
    {
        if (change.JustDied(health))
            LoseGame();
    }
    
    protected abstract void LoseGame();
    protected abstract void WinGame();
    
    protected void OnRestartPressed(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
