using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(ThirdPersonCamera))]
public class PlayerInputController : MonoBehaviour
{
    
    #region Private Variables

    // -- Input Actions -------------------------------
    private InputAction _movementAction;
    private InputAction _attackAction;
    private InputAction _lookAction;
    
    // -- Components ----------------------------------
    private PlayerMovement _playerMovement;
    private ThirdPersonCamera _tpCamera;

    #endregion

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _tpCamera = GetComponent<ThirdPersonCamera>();
    }

    void Start()
    {
        _movementAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _attackAction = InputSystem.actions.FindAction("Jump");
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
}
