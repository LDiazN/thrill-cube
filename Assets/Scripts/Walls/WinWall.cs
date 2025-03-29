using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

public class WinWall : MonoBehaviour
{
    #region Inspector Properties
    [FormerlySerializedAs("gameManager")] [Description("Game manager controlling the flow of the game")]
    public KillAllEnemiesGameManager killAllEnemiesGameManager;
    #endregion
    
    #region Events

    public event Action OnPlayerReach;
    
    #endregion

    private void Reset()
    {
        killAllEnemiesGameManager = FindFirstObjectByType<KillAllEnemiesGameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        
        OnPlayerReach?.Invoke();
    }
}
