using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] private float _maxSpeed = 1f;

    #endregion

    #region Internal State

    // -- Components ----------------------------------------
    private Rigidbody _rigidbody;

    // -- Internal Variables ----------------------------------------
    /// <summary>
    /// Vector with a size in [0,1] with the movement direction. X means
    /// horizontal movement (strafe). And Y means forward movement
    /// </summary>
    private Vector3 _movementDirection = Vector3.zero;

    public Vector3 MovementDirection
    {
        get => _movementDirection;
        set => _movementDirection = value;
    }

    private Vector3 _lookDirection;

    public Vector3 LookDirection
    {
        get => _lookDirection;
        set => _lookDirection = value;
    }

    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _lookDirection = transform.forward;
    }

    private void Update()
    {
        UpdateRotation();
        Move();
    }

    private void UpdateRotation()
    {
        
    }

    private void Move()
    {
        _rigidbody.linearVelocity = _movementDirection.z * _maxSpeed * transform.forward +
                                    _movementDirection.x * _maxSpeed * transform.right;
    }
}