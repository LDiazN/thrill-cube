using UnityEngine;

public abstract class BaseUIComponent : MonoBehaviour
{
    #region Related Game Objects
    GameManager gameManager;
    #endregion
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void OnEnable()
    {
        if (gameManager != null)
            RegisterEvents(gameManager);
    }
    
    private void OnDisable()
    {
        if (gameManager != null)
            DeregisterEvents(gameManager);
    }

    protected virtual void RegisterEvents(GameManager gameManager)
    {
        gameManager.OnLose += OnLose;
        gameManager.OnWin += OnWin;
    }

    protected virtual void DeregisterEvents(GameManager gameManager)
    {
        gameManager.OnLose -= OnLose;
        gameManager.OnWin -= OnWin;
    }

    protected virtual void OnLose()
    {
        
    }
    
    protected virtual void OnWin()
    {
        
    }

}
