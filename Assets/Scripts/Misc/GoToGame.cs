using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DeathWallGameManager))]
public class GoToGame : MonoBehaviour
{

    #region Inspector Properties
    [SerializeField] private string gameSceneName;
    #endregion
    
    #region Components
    DeathWallGameManager deathWallGameManager;
    #endregion
    private void Awake()
    {
        deathWallGameManager = GetComponent<DeathWallGameManager>();
        deathWallGameManager.OnWin += GoGame;
    }

    private void GoGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
