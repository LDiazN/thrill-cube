using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(ThirdPersonCamera), typeof(Shoot))]
public class PlayerInputController : PlayerController
{
    
    #region Private Variables

    // -- Input Actions -------------------------------
    private InputAction _movementAction;
    private InputAction _attackAction;
    private InputAction _lookAction;
    private InputAction _throwAction;
    private InputAction _interactAction;
    private InputAction _changeToAIAction;
    private bool _callbacksRegistered;
    
    #endregion
    
    # region Components

    private Player _player;

    #endregion

    private void Awake()
    {
        _movementAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _throwAction = InputSystem.actions.FindAction("SecondaryAttack");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _interactAction = InputSystem.actions.FindAction("Interact");

        _changeToAIAction = InputSystem.actions.FindAction("ChangeAI");
        
        _player = GetComponent<Player>();
    }

    void Start()
    {
        RegisterCallbacks();
    }

    private void OnEnable()
    {
        RegisterCallbacks();
    }

    private void OnDisable()
    {
        DeregisterCallbacks();
    }

    void Update()
    {
        UpdatePlayerMovement();
        UpdateCameraMovement();
    }

    void RegisterCallbacks()
    {
        if (_callbacksRegistered)
            return;
        
        _attackAction.performed += UpdateShoot;
        _throwAction.performed += UpdateThrow;
        _interactAction.performed += UpdatePick;
        _changeToAIAction.performed += ChangeToAI;
        _callbacksRegistered = true;
    }
    
    void DeregisterCallbacks()
    {
        if (_attackAction != null)
            _attackAction.performed -= UpdateShoot;
        if (_throwAction != null)
            _throwAction.performed -= UpdateThrow;
        if (_interactAction != null)
            _interactAction.performed -= UpdatePick;
        if (_changeToAIAction != null)
            _changeToAIAction.performed -= ChangeToAI;
        
        _callbacksRegistered = false;
    }

    void UpdateCameraMovement()
    {
        var lookOffset = _lookAction.ReadValue<Vector2>();
        _player.TPSCamera.CameraMovement = lookOffset;
    }
    
    void UpdatePlayerMovement()
    {
        var movementDirection = GetMovementDirection();
        _player.PlayerMovement.MovementDirection = movementDirection;
    }

    Vector3 GetMovementDirection()
    {
        Vector2 movementInput = _movementAction.ReadValue<Vector2>();
        Vector3 movementDirection = new Vector3(movementInput.x, 0, movementInput.y);
        movementDirection.Normalize();
        return movementDirection;
    }

    void UpdateShoot(InputAction.CallbackContext context)
    {
        if (_player.Equipment.currentGun)
            _player.Shoot.Fire();
        else if (_player.Equipment.currenThrowable)
            _player.Throw.ThrowEquipment();
        else if (!_player.Equipment.HasEquipment)
            TryPick();
    }

    void UpdateThrow(InputAction.CallbackContext context)
    {
        Debug.Log("Trying to throw");
        _player.Throw?.ThrowEquipment();
    }

    void UpdatePick(InputAction.CallbackContext context)
    {
       Debug.Log("Trying to pick");
       TryPick();
    }

    private void TryPick()
    {
       if (!_player.Equipment.TryEquipFromPicker())
           _player.Equipment.TryEquipFromFloor();
    }

    private void ChangeToAI(InputAction.CallbackContext context)
    {
        _player.ChangeToAI(!_player.UsingAI);
    }
}