using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    #region Inspector Properties
    [SerializeField] string sampleSceneName;
    [SerializeField] string knowledgeShareScene;
    [SerializeField] string shootingEnemyScene;
    [SerializeField] string mainMenuScene;
    #endregion 
    
    #region Internal State

    private InputAction _loadMainMenu;

    #endregion

    private void Start()
    {
        _loadMainMenu = InputSystem.actions.FindAction("GoMainMenu");
        _loadMainMenu.performed += GoMainMenu;
    }

    private void OnDisable()
    {
        if (_loadMainMenu != null)
            _loadMainMenu.performed -= GoMainMenu;
    }


    private void GoMainMenu(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void GoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoSampleScene()
    {
        GoScene(sampleSceneName);
    }

    public void GoKnowledgeShareScene()
    {
        GoScene(knowledgeShareScene);
    }

    public void GoShootingEnemyScene()
    {
        GoScene(shootingEnemyScene);
    }
}
