using UnityEngine;

public class WinScreen : BaseUIComponent
{
    #region Inspector Properties
    [SerializeField] private GameObject _gameOverScreen;
    #endregion
    protected override void OnWin()
    {
        _gameOverScreen.SetActive(true);
    }
}