using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class AIInputController : MonoBehaviour
{
    #region Inspector Properties

    [Description("This object will implement the actual player AI, while this script translates that behaviour to " +
                 "player commands")]
    [SerializeField] private AIGuide aiGuidePrefab;
    
    [SerializeField] private float maxGuideDistance = 3;
    
    [Description("If the player is closer than this distance to the AI guide, it won't move")]
    [SerializeField] private float desiredDistanceToGuide = 1;

    private float _desiredDistanceSqrd => desiredDistanceToGuide * desiredDistanceToGuide;
    
    
    #endregion
    
    #region Components

    private Player _player;
    
    #endregion
    
    #region Internal State

    private InputAction _changeToAIAction;
    private bool _callbacksRegistered = false;
    [CanBeNull] private AIGuide _aiGuide;

    #endregion
    private void Awake()
    {
        _changeToAIAction = InputSystem.actions.FindAction("ChangeAI");
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        RegisterCallbacks();
    }
    
    private void Start()
    {
        // Don't delete this, we need this both in OnEnable and Start
        RegisterCallbacks();
    }

    private void OnDisable()
    {
       DeregisterCallbacks();
       DeactivateGuide();
    }

    private void Update()
    {
        // TrySyncGuide();
        UpdatePlayerMovement();
        UpdateCameraMovement();
    }

    private void OnDrawGizmos()
    {
        if (!_player)
            return; 
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + _player.PlayerMovement.MovementDirection);
    }

    void UpdateCameraMovement()
    {
        // _player.TPSCamera.CameraMovement = lookOffset;
        var guide = GetAIGuide();
        var enemy = guide.GetClosestEnemy();
        Vector3 targetPosition;
        LayerMask ignoreMask = 1 << LayerMask.NameToLayer("Player");
        if (enemy && guide.RangeDetector.HasLineOfSight(enemy, ignoreMask))
            targetPosition = enemy.transform.position;
        else
            targetPosition = guide.transform.position;

        var desired = targetPosition - _player.TPSCamera.GetCameraPosition();
        var current = _player.TPSCamera.GetCameraDirection();
        var change = Quaternion.FromToRotation(current, desired).eulerAngles;
        var xRotation = change.x;
        var yRotation = change.y;

        _player.TPSCamera.CameraMovement = new( xRotation,  yRotation);

    }
    
    void UpdatePlayerMovement()
    {
        var guide = GetAIGuide();
        var movementDirection = guide.transform.position - transform.position;
        
        if (movementDirection.sqrMagnitude < _desiredDistanceSqrd)
            movementDirection = Vector3.zero;
        else if (movementDirection.sqrMagnitude > 1) // if magnitude is less than one, no need to normalize
            movementDirection.Normalize();

        movementDirection.y = 0;
        
        _player.PlayerMovement.MovementDirection = transform.worldToLocalMatrix * movementDirection;
    }

    void TrySyncGuide()
    {
        var guide = GetAIGuide();
        var toGuide = guide.transform.position - transform.position;
        var distance = toGuide.magnitude;
        
        if (distance > maxGuideDistance)
            SyncGuide();
    }
    void SyncGuide()
    {
        var guide = GetAIGuide();
        guide.transform.position = transform.position;
        guide.transform.rotation = transform.rotation;
    }

    private AIGuide GetAIGuide()
    {
        if (!_aiGuide)
            _aiGuide = Instantiate(aiGuidePrefab, transform.position, transform.rotation);
        else if (!_aiGuide.gameObject.activeInHierarchy)
            _aiGuide.gameObject.SetActive(true);

        return _aiGuide;
    }

    private void DeactivateGuide()
    {
        if (_aiGuide)
            _aiGuide.gameObject.SetActive(false);
    }

    private void ChangeToAI(InputAction.CallbackContext context)
    {
        _player = GetComponent<Player>();
        _player.ChangeToAI(!_player.UsingAI);
    }

    private void RegisterCallbacks()
    {
        if (_callbacksRegistered)
            return;

        _changeToAIAction.performed += ChangeToAI;

        _callbacksRegistered = true;
    }

    private void DeregisterCallbacks()
    {
        _changeToAIAction.performed -= ChangeToAI;
        _callbacksRegistered = false;
    }
}
