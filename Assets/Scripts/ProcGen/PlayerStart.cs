using System;
using UnityEngine;

/// <summary>
/// Use this to mark where the player starts. It will take over the main
/// camara in scene to place the player
/// </summary>
public class PlayerStart : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField]
    private Player player;

    #endregion
    
    #region Events

    public event Action OnPlayerSpawned;
    
    #endregion

    private void Awake()
    {
        player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
    }

    public void SpawnPlayer()
    {
        if (!player)
            return;
        
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
        player.gameObject.SetActive(true);
        OnPlayerSpawned?.Invoke();
        gameObject.SetActive(false);
    }
}
