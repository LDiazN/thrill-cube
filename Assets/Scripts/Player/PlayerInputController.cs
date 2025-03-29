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
    
    #endregion
    
    # region Components

    private Player _player;

    #endregion

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Start()
    {
        _movementAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _throwAction = InputSystem.actions.FindAction("SecondaryAttack");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _interactAction = InputSystem.actions.FindAction("Interact");
        
        _attackAction.performed += UpdateShoot;
        _throwAction.performed += UpdateThrow;
        _interactAction.performed += UpdatePick;
    }

    private void OnDisable()
    {
        if (_attackAction != null)
            _attackAction.performed -= UpdateShoot;
        if (_throwAction != null)
            _throwAction.performed -= UpdateThrow;
    }

    void Update()
    {
        UpdatePlayerMovement();
        UpdateCameraMovement();
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
        _player.Shoot.Fire();
    }

    void UpdateThrow(InputAction.CallbackContext context)
    {
        Debug.Log("Trying to throw");
        _player.Throw?.ThrowEquipment();
    }

    void UpdatePick(InputAction.CallbackContext context)
    {
       Debug.Log("Trying to pick");
       _player.Equipment.TryEquipFromPicker();
    }
}
