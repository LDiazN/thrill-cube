using UnityEngine;
using UnityEngine.InputSystem;

public class AIInputController : MonoBehaviour
{
    #region Components

    private Player _player;
    
    #endregion
    
    #region Internal State

    private InputAction _changeToAIAction;
    private bool _callbacksRegistered = false;
    
    #endregion
    private void Awake()
    {
        _changeToAIAction = InputSystem.actions.FindAction("ChangeAI");
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        RegisterCallbacks();
    }
    
    private void Start()
    {
        // Don't delete this, we need this both in OnEnable and Start
        RegisterCallbacks();
    }

    private void OnDisable()
    {
       DeregisterCallbacks();
    }

    private void ChangeToAI(InputAction.CallbackContext context)
    {
        _player = GetComponent<Player>();
        _player.ChangeToAI(!_player.UsingAI);
    }

    private void RegisterCallbacks()
    {
        if (_callbacksRegistered)
            return;

        _changeToAIAction.performed += ChangeToAI;

        _callbacksRegistered = true;
    }

    private void DeregisterCallbacks()
    {
        _changeToAIAction.performed -= ChangeToAI;
        _callbacksRegistered = false;
    }
}
