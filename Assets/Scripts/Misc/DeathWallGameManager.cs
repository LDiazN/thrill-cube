using System.ComponentModel;
using UnityEngine;

public class DeathWallGameManager : GameManager
{
    #region Inspector Properties
    [Description("Win wall in this map")]
    [SerializeField]
    private WinWall wall;
    
    [Description("Player in the game")]
    [SerializeField]
    private Player player;
    #endregion

    private void Reset()
    {
        wall = FindFirstObjectByType<WinWall>();
        player = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        wall.OnPlayerReach += WinGame;
        
        if (_player)
            player.Health.OnHealthChanged += PlayerHealthChanged;
        
        _restartAction.performed += OnRestartPressed;
    }

    protected override void WinGame()
    {
        
    }

    protected override void LoseGame()
    {
        _player.DeactivateInput();
        _gameState = GameState.Lose;
        CallLose();
    }
}
