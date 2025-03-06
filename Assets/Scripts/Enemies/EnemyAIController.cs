using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;

[RequireComponent(typeof(Health), typeof(MeshRenderer))]
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

    private void OnDead(Health health, int offset, Vector3 direction)
    {
        if (!health.isDead)
            return;
        
        _rigidbody.constraints = RigidbodyConstraints.None;
        // When it dies the force is stronger
        var force = 10;
        if (offset < 0)
            force *= 3;
        if (offset < 0)
            _forceRequests.Enqueue(direction * force);
        
        // Make it smaller 
        var newScale = transform.localScale;
        newScale *= 0.1f;
        transform.DOScale(newScale, 1f).OnComplete(
            () => { _renderer.material = _deadEnemyMaterial;}
        ).Play();

    }

    private void OnHurt(Health health, int offset, Vector3 direction)
    {
        _rigidbody.AddForce(7 * direction, ForceMode.Impulse);
        OnDead(health, offset, direction);
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
