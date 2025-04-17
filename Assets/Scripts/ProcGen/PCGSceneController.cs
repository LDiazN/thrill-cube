using System;
using UnityEngine;

public class PCGSceneController : MonoBehaviour
{
    #region Internal State

    private RoomGen _roomGen;
    
    #endregion

    private void Awake()
    {
        _roomGen = FindFirstObjectByType<RoomGen>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _roomGen.Reset();
            _roomGen.GenerateRooms();
        }
    }
}
