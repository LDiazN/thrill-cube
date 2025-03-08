using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody), typeof(ThirdPersonCamera))]
public class PlayerMovement : MonoBehaviour
{
    #region Inspector Variables

    [FormerlySerializedAs("_maxSpeed")] [SerializeField] private float maxSpeed = 1f;

    #endregion

    #region Internal State

    // -- Components ----------------------------------------
    private Rigidbody _rigidbody;
    private ThirdPersonCamera _tpCamera;

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

    private bool _canMove = true;
    public bool CanMove { get => _canMove; set => _canMove = value; }
    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _tpCamera = GetComponent<ThirdPersonCamera>();
    }

    private void FixedUpdate()
    {
        _tpCamera.UpdateThirdPersonCamera();
        UpdateRotation();
        Move();
    }

    private void UpdateRotation()
    {
        _rigidbody.angularVelocity = Vector3.zero;
        if (Mathf.Abs(_movementDirection.z) < 0.01f && Mathf.Abs(_movementDirection.x) < 0.01f)
            return;
        
        var originalFollowRotation = _tpCamera.FollowTarget.transform.rotation;
        var desiredRotation = Quaternion.LookRotation(_lookDirection, Vector3.up).eulerAngles;
        
        // _rigidbody.rotation = Quaternion.Euler(desiredRotation);    
        transform.rotation = Quaternion.Euler(desiredRotation);
        
        // Restore follow target rotation bc it rotates with the player
        var followRotation = _tpCamera.transform.rotation.eulerAngles;
        _tpCamera.FollowTarget.transform.localRotation = Quaternion.Euler(followRotation.x, 0, 0);
        
        _tpCamera.FollowTarget.transform.rotation = originalFollowRotation;
    }

    private void Move()
    {
        if (!_canMove)
            return;
        
        var movement = _movementDirection.z * maxSpeed * transform.forward +
                       _movementDirection.x * maxSpeed * transform.right;
        _rigidbody.linearVelocity = movement;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Ray(transform.position, _lookDirection));
    }
}