using System;
using TMPro;
using UnityEngine;

public class EnemyCounter : BaseUIComponent
{
    #region Inspector Properties
    
    [Header("My own components")]
    public TextMeshProUGUI enemyCounter;
    
    #endregion
    
    void SetEnemyCount(int value)
    {
        enemyCounter.text = value.ToString();
    }

    protected override void OnLose()
    {
        gameObject.SetActive(false);
    }

    protected override void OnWin()
    {
        gameObject.SetActive(false);
    }

    protected override void RegisterEvents(GameManager gameManager)
    {
        base.RegisterEvents(gameManager);
        gameManager.OnEnemyCountChanged += SetEnemyCount;
    }

    protected override void DeregisterEvents(GameManager gameManager)
    {
        base.DeregisterEvents(gameManager);
        gameManager.OnEnemyCountChanged -= SetEnemyCount;
    }
}
