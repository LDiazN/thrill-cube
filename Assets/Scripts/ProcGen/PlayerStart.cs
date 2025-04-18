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
    private GameObject playerPrefab;

    #endregion
    
    #region Internal State

    private Camera mainCamera;

    #endregion


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (mainCamera)
            Destroy(mainCamera.gameObject);

        Instantiate(playerPrefab, transform.position, transform.rotation);
    }
    
    
}
