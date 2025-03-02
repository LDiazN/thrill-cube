using System;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using UnityEngine.Serialization;

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

    #endregion

    private void Start()
    {
        _originalOffset = playerCamera.transform.localPosition;
    }

    private void Update()
    {
        UpdateCameraRotation();
        UpdateCameraTilt();
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
}