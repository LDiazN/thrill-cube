using UnityEngine;

public class BSPDemoCamera : MonoBehaviour
{ 
    #region Inspector Properties 
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float distance = 10f;
    [SerializeField]
    private float zoomSpeed = 2f;
    [SerializeField]
    private float minDistance = 2f;
    [SerializeField]
    private float maxDistance = 20f;

    [SerializeField]
    private float orbitSpeed = 10f;
    [SerializeField]
    private float dragSensitivity = 4f;

    
    [SerializeField]
    private float momentumDamping = 5f; // affects both rotation & zoom momentum
    [SerializeField]
    private float idleDelay = 3f;
    #endregion

    #region  Internal State

    private float _yaw;
    private float _pitch;

    private float _idleTimer = 0f;

    private Vector3 _previousMousePosition;
    private Vector2 _rotationVelocity;
    private float _zoomVelocity;

    private bool _isDragging = false;
    private bool _isZooming = false;

    #endregion

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("BSPDemoCamera: No target assigned!");
            enabled = false;
            return;
        }

        Vector3 offset = transform.position - target.position;
        distance = offset.magnitude;
        _yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        _pitch = Mathf.Asin(offset.y / distance) * Mathf.Rad2Deg;
    }

    void Update()
    {
        HandleInput();
        ApplyMomentum();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        // Zoom input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            _zoomVelocity = -scroll * zoomSpeed;
            _isZooming = true;
            _idleTimer = 0f;
        }

        // Drag input
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mouseDelta = Input.mousePosition - _previousMousePosition;

            float deltaYaw = mouseDelta.x * dragSensitivity * Time.deltaTime;
            float deltaPitch = -mouseDelta.y * dragSensitivity * Time.deltaTime;

            _yaw += deltaYaw;
            _pitch += deltaPitch;
            _pitch = Mathf.Clamp(_pitch, -85f, 85f);

            _rotationVelocity = new Vector2(deltaYaw, deltaPitch);

            _isDragging = true;
            _idleTimer = 0f;
        }
        else
        {
            if (_isDragging)
            {
                _isDragging = false;
                _idleTimer = 0f;
            }
            else
            {
                _idleTimer += Time.deltaTime;

                if (_idleTimer > idleDelay)
                {
                    _yaw += orbitSpeed * Time.deltaTime;
                }
            }
        }

        _previousMousePosition = Input.mousePosition;
    }

    void ApplyMomentum()
    {
        // Orbit momentum
        if (!_isDragging && _idleTimer < idleDelay)
        {
            _yaw += _rotationVelocity.x;
            _pitch += _rotationVelocity.y;
            _pitch = Mathf.Clamp(_pitch, -85f, 85f);

            _rotationVelocity = Vector2.Lerp(_rotationVelocity, Vector2.zero, momentumDamping * Time.deltaTime);
        }

        // Zoom momentum
        if (!_isZooming || Mathf.Abs(_zoomVelocity) > 0.001f)
        {
            distance += _zoomVelocity * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            _zoomVelocity = Mathf.Lerp(_zoomVelocity, 0f, momentumDamping * Time.deltaTime);
        }

        _isZooming = false;
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 direction = rotation * Vector3.forward;
        transform.position = target.position - direction * distance;
        transform.LookAt(target);
    }
}
