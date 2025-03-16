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
    
    // -- Components ----------------------------------
    private PlayerMovement _playerMovement;
    private ThirdPersonCamera _tpCamera;
    private Shoot _shoot;

    #endregion

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _tpCamera = GetComponent<ThirdPersonCamera>();
        _shoot = GetComponent<Shoot>();
    }

    void Start()
    {
        _movementAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _attackAction.performed += UpdateShoot;
    }

    private void OnDisable()
    {
        _attackAction.performed -= UpdateShoot;
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
}
