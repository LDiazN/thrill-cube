using System;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using UnityEngine.Serialization;
using Color = UnityEngine.Color;

[RequireComponent(typeof(PlayerMovement))]
public class ThirdPersonCamera : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] private Camera playerCamera;

    [Description("How fast the camera rotates around the player, in degrees")] [SerializeField]
    private float cameraRotationSpeed = 180;

    [Description("How fast the camera tilts around the player, in degrees")] [SerializeField]
    private float cameraTiltSpeed = 180;

    [Description("Target used for the third person follow camera")]
    [SerializeField] private GameObject followTarget;
    public GameObject FollowTarget => followTarget;
    
    [Description("How much to tilt the camera to emulate head movement on horizontal movement")]
    [SerializeField] private float maxTiltAngle = 10;

    #endregion

    #region Internal State

    /// <summary>
    /// (rotation, tilt)
    /// </summary>
    private Vector2 _cameraMovement = Vector2.zero;

    public Vector2 CameraMovement
    {
        get => _cameraMovement;
        set => _cameraMovement = value;
    }

    /// <summary>
    /// Original position that the camera had from the player
    /// </summary>
    private Vector3 _originalOffset;

    private PlayerMovement _playerMovement;

    private float _tiltSmoothVelocity = 0;
    
    #endregion

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _originalOffset = playerCamera.transform.localPosition;
    }

    // Called by player movement component in fixed update
    public void UpdateThirdPersonCamera()
    {
        UpdateCameraRotation();
        UpdateCameraTilt();
        UpdatePlayerLookDirection();
        UpdateHeadTilt();
    }

    private void UpdateCameraRotation()
    {
        followTarget.transform.Rotate(Vector3.up,
            _cameraMovement.x * cameraRotationSpeed * Time.fixedDeltaTime);
    }

    private void UpdateCameraTilt()
    {
        // We only care about the X axis rotation
        followTarget.transform.rotation *= Quaternion.AngleAxis(-cameraTiltSpeed * Time.fixedDeltaTime * _cameraMovement.y, Vector3.right);

        var angles = followTarget.transform.localEulerAngles;
        angles.z = 0;
        var angle = angles.x;

        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }
        
        followTarget.transform.localEulerAngles = angles;
    }

    private void UpdatePlayerLookDirection()
    {
        // project camera target forward direction to know where the player 
        // should be looking at
        var direction = followTarget.transform.forward;
        direction.y = 0;
        direction.Normalize();
        _playerMovement.LookDirection = direction;
    }

    /// <summary>
    /// A vector pointing in the direction of the camera
    /// </summary>
    public Vector3 GetCameraDirection()
    {
        return Camera.main.transform.forward;
    }

    public Vector3 GetCameraPosition()
    {
        return Camera.main.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(GetCameraPosition(), GetCameraPosition() + 1000 * GetCameraDirection());
    }

    private void UpdateHeadTilt()
    {
        var rotation = followTarget.transform.rotation.eulerAngles;
        var desiredRotation = - _playerMovement.MovementDirection.x * maxTiltAngle;
        rotation.z = Mathf.SmoothDamp(rotation.z, desiredRotation, ref _tiltSmoothVelocity, 0.2f);
        followTarget.transform.rotation = Quaternion.Euler(rotation);
    }

    /// <summary>
    /// Return the position of the object being pointed by the camera.
    /// The reticle's target
    ///
    /// If no target, will return a position at the infinite in the direction of the camera
    /// </summary>
    /// <returns>Position of the thing the camera is seeing right now</returns>
    public Vector3 GetTarget()
    {
        // Try to shoot at  target very far away. If you find something else first, shoot at that 
        var hitSomething = Physics.Raycast(GetCameraPosition(), GetCameraDirection(), out RaycastHit hit);
        if (hitSomething) 
            return hit.point;
        
        // Didn't find anything, shoot at the infinite void
        return GetCameraPosition() + 1000 * GetCameraDirection();
    }
}