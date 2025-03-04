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
    }

    private void UpdateCameraRotation()
    {
        followTarget.transform.Rotate(Vector3.up,
            _cameraMovement.x * cameraRotationSpeed * Time.deltaTime);
    }

    private void UpdateCameraTilt()
    {
        // We only care about the X axis rotation
        followTarget.transform.rotation *= Quaternion.AngleAxis(- cameraTiltSpeed * Time.deltaTime * _cameraMovement.y, Vector3.right);

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
}