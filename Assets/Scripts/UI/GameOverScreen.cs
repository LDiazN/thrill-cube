using Unity.VisualScripting;
using UnityEngine;

public class GameOverScreen : BaseUIComponent
{
    #region Inspector Properties
    [SerializeField] private GameObject _gameOverScreen;
    #endregion
    protected override void OnLose()
    {
        _gameOverScreen.SetActive(true);
    }
}
