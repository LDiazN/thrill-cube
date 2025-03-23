using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using Unity.Behavior;
using Unity.AI;
using UnityEngine.AI;
using Action = System.Action;

[RequireComponent(typeof(Health), typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    #region Inspector Properties

    [Header("Visuals")]
    
    [Description("Used when the enemy is dead")]
    [SerializeField] private Material _deadEnemyMaterial;

    [Description("A multiplier added to the knockback of an enemy, to make it more heavy or light")]
    [Min(0)]
    [SerializeField] private float _knockbackMultiplier = 1;
    
    [SerializeField]
    MeshRenderer _renderer;
    
    
    #endregion
    
    #region Internal State
    
    Queue<Vector3> _forceRequests = new();
    
    #endregion
    
    #region Components
    
    Health _health;
    Rigidbody _rigidbody;
    BehaviorGraphAgent _bgAgent;
    NavMeshAgent _navAgent;
    
    public Health Health => _health;
    
    #endregion

    #region Callbacks

    public event Action OnJustDied;
    
    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();
        _bgAgent = GetComponent<BehaviorGraphAgent>();
        _navAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _health.OnHealthChanged += OnHurt;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= OnHurt;
    }

    /// <summary>
    /// Called when you shoot a dead enemy (just died or already dead)
    /// </summary>
    /// <param name="health">Affected heatlh component</param>
    /// <param name="change">Object specifying the change in health event</param>
    private void OnDead(Health health, Health.Change change)
    {
        if (!health.isDead)
            return;
        
        _rigidbody.constraints = RigidbodyConstraints.None;
        // When it dies the force is stronger
        var force = change.knockback;
        if (change.IsDamage)
        {
            force = change.knockbackOnDead;
        }
        _forceRequests.Enqueue(change.direction * force);
        
        // Make it smaller 
        var newScale = transform.localScale;
        newScale *= 0.9f;
        transform.DOScale(newScale, 1f).OnComplete(
            () => { _renderer.material = _deadEnemyMaterial;}
        ).Play();
        
        // Shutdown the AI
        _bgAgent.enabled = false;
        _navAgent.enabled = false;
        
        // Report this enemy as dead
        if (change.JustDied(health))
            OnJustDied?.Invoke(); 
    }

    private void OnHurt(Health health, Health.Change change)
    {
        // Apply knockback to enemy
        _rigidbody.AddForce(change.knockback * _knockbackMultiplier * change.direction, ForceMode.Impulse);
        OnDead(health, change);
    }

    private void FixedUpdate()
    {
        // Apply all enqueued force requests
        while (_forceRequests.Count > 0)
        {
            var force = _forceRequests.Dequeue();
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
