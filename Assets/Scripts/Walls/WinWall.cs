using System;
using UnityEngine;

public class WinWall : MonoBehaviour
{
    #region Events

    public event Action OnPlayerReach;
    
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        
        OnPlayerReach?.Invoke();
    }
}
