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
        UpdateCameraMovement();
        UpdatePlayerMovement();
        UpdateShoot();
    }

    private void UpdateShoot()
    {
        var aiGuide = GetAIGuide();
        if (!aiGuide.ShouldShoot)
            return;
        
        _player.TryAttack();
    }

    private void OnDrawGizmos()
    {
        if (!_player)
            return; 
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + _player.PlayerMovement.MovementDirection);

        if (!_aiGuide)
            return;
        var closestEnemy = _aiGuide.GetClosestEnemy();
        if (!closestEnemy)
            return;
        
        Gizmos.color = Color.yellow;
        var cameraPos = _player.TPSCamera.GetCameraPosition();
        
        Gizmos.DrawLine(cameraPos, closestEnemy.transform.position);
    }

    void UpdateCameraMovement()
    {
        var guide = GetAIGuide();
        var enemy = guide.GetClosestEnemy();
        Vector3 targetPosition;
        LayerMask ignoreMask = 1 << LayerMask.NameToLayer("Player");
        
        // Choose the direction to point to depending on whether we have line of sight to the enemy
        // or not
        if (enemy && guide.ShootingDistance.HasLineOfSight(enemy, ignoreMask))
            targetPosition = enemy.transform.position + Vector3.up;
        else
            targetPosition = guide.transform.position + 10 * guide.transform.forward;

        // Where we want the character looking at 
        var desiredDir = targetPosition - _player.TPSCamera.GetCameraPosition();
        var currentDir = _player.TPSCamera.GetCameraDirection();
        
        // Compute the rotation required to get to the desired direction
        var change = GetXYRotToAlign(currentDir, desiredDir);
        
        // This is inverted bc in a mouse horizontal movement rotates around the Y axis, while vertical movement
        // rotates the X axis 
        _player.TPSCamera.CameraMovement = new(change.y,change.x);
    }
    
    private static Vector2 GetXYRotToAlign(Vector3 currentDir, Vector3 targetDir)
    {
        currentDir.Normalize();
        targetDir.Normalize();

        // Compute current yaw and pitch
        float currentYaw   = Mathf.Atan2(currentDir.x, currentDir.z) * Mathf.Rad2Deg;
        float currentPitch = Mathf.Asin(currentDir.y) * Mathf.Rad2Deg;

        // Compute target yaw and pitch
        float targetYaw   = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
        float targetPitch = Mathf.Asin(targetDir.y) * Mathf.Rad2Deg;

        // Calculate deltas using shortest angle difference
        float deltaYaw   = Mathf.DeltaAngle(currentYaw, targetYaw);
        float deltaPitch = Mathf.DeltaAngle(currentPitch, targetPitch);

        return new Vector2(deltaPitch, deltaYaw); // X = pitch, Y = yaw
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

        if (_aiGuide)
            _aiGuide.player = _player;
        
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
