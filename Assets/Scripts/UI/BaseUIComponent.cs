using UnityEngine;

public abstract class BaseUIComponent : MonoBehaviour
{
    #region Related Game Objects
    KillAllEnemiesGameManager _killAllEnemiesGameManager;
    #endregion
    
    private void Awake()
    {
        _killAllEnemiesGameManager = FindFirstObjectByType<KillAllEnemiesGameManager>();
    }
    
    private void OnEnable()
    {
        if (_killAllEnemiesGameManager != null)
            RegisterEvents(_killAllEnemiesGameManager);
    }
    
    private void OnDisable()
    {
        if (_killAllEnemiesGameManager != null)
            DeregisterEvents(_killAllEnemiesGameManager);
    }

    protected virtual void RegisterEvents(KillAllEnemiesGameManager killAllEnemiesGameManager)
    {
        killAllEnemiesGameManager.OnLose += OnLose;
        killAllEnemiesGameManager.OnWin += OnWin;
    }

    protected virtual void DeregisterEvents(KillAllEnemiesGameManager killAllEnemiesGameManager)
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
