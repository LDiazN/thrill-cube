using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;

[RequireComponent(typeof(Health), typeof(MeshRenderer), typeof(Rigidbody))]
public class EnemyAIController : MonoBehaviour
{
    #region Inspector Properties

    [Description("Used when the enemy is dead")]
    [SerializeField] private Material _deadEnemyMaterial;
    #endregion
    #region Internal State
    Health _health;
    Rigidbody _rigidbody;
    MeshRenderer _renderer;
    
    Queue<Vector3> _forceRequests = new();
    #endregion


    private void Awake()
    {
        _health = GetComponent<Health>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _health.OnHealthChanged += OnHurt;
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

    }

    private void OnHurt(Health health, Health.Change change)
    {
        // Apply knockback to enemy
        _rigidbody.AddForce(change.knockback * change.direction, ForceMode.Impulse);
        OnDead(health, change);
    }

    private void FixedUpdate()
    {
        while (_forceRequests.Count > 0)
        {
            var force = _forceRequests.Dequeue();
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
