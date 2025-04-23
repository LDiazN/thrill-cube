using System;
using UnityEngine;

public class DemoSceneGameManager : MonoBehaviour
{
    #region Inspector Properties

    [Min(0)]
    [SerializeField] private float timeBetweenGenerations = 2;
    [SerializeField] private bool autoGenerationActive = true;

    #endregion

    #region Internal State

    private RoomGen _roomGen;
    private float _timeSinceLastGeneration = 1000;

    #endregion

    private void Awake()
    {
        _roomGen = FindFirstObjectByType<RoomGen>();
    }


    private void Update()
    {
        _timeSinceLastGeneration += Time.deltaTime;
        if (_timeSinceLastGeneration > timeBetweenGenerations && autoGenerationActive)
        {
            RegenerateRooms();
            _timeSinceLastGeneration = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
            RegenerateRooms();
    }

    private void RegenerateRooms()
    {
        _roomGen.Reset();
        _roomGen.GenerateRooms();
    }
    
    public void ToggleAutoGeneration()
    {
        autoGenerationActive = !autoGenerationActive;
    }
    
    public void ToggleStaticsGenerationActive()
    {
        _roomGen.generateStatics = !_roomGen.generateStatics;
    }

    public void ToggleEnemiesGenerationActive()
    {
        _roomGen.generateEnemies = !_roomGen.generateEnemies;
    }
    
    public void TogglePickablesGenerationActive()
    {
        _roomGen.generatePickables = !_roomGen.generatePickables ;
    }
}
