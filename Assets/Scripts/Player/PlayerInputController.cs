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
    
    #endregion
    
    # region Components
    
    private PlayerMovement _playerMovement;
    private ThirdPersonCamera _tpCamera;
    private Shoot _shoot;
    private Throw _throw;

    #endregion

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _tpCamera = GetComponent<ThirdPersonCamera>();
        _shoot = GetComponent<Shoot>();
        _throw = GetComponent<Throw>();
    }

    void Start()
    {
        _movementAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _throwAction = InputSystem.actions.FindAction("SecondaryAttack");
        _attackAction = InputSystem.actions.FindAction("Attack");
        
        _attackAction.performed += UpdateShoot;
        _throwAction.performed += UpdateThrow;
        
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
        _tpCamera.CameraMovement = lookOffset;
    }
    
    void UpdatePlayerMovement()
    {
        var movementDirection = GetMovementDirection();
        _playerMovement.MovementDirection = movementDirection;
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
        _shoot.Fire();
    }

    void UpdateThrow(InputAction.CallbackContext context)
    {
        Debug.Log("Trying to throw");
        _throw?.ThrowEquipment();
    }
}
