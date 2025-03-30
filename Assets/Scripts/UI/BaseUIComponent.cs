using UnityEngine;

public abstract class BaseUIComponent : MonoBehaviour
{
    #region Related Game Objects
    GameManager _gameManager;
    #endregion
    
    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }
    
    private void OnEnable()
    {
        if (_gameManager != null)
            RegisterEvents(_gameManager);
    }
    
    private void OnDisable()
    {
        if (_gameManager != null)
            DeregisterEvents(_gameManager);
    }

    protected virtual void RegisterEvents(GameManager gameManager)
    {
        gameManager.OnLose += OnLose;
        gameManager.OnWin += OnWin;
    }

    protected virtual void DeregisterEvents(GameManager killAllEnemiesGameManager)
    {
        killAllEnemiesGameManager.OnLose -= OnLose;
        killAllEnemiesGameManager.OnWin -= OnWin;
    }

    protected virtual void OnLose()
    {
        
    }
    
    protected virtual void OnWin()
    {
        
    }

}
