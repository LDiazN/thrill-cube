using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Throwable : MonoBehaviour
{
    #region Inspector Variables

    [Description("How much damage does this object do when hitting an enemy")] [SerializeField]
    private int damage;
    
    [Description("How much knockback does this object do when hitting an enemy")] [SerializeField]
    private float knockback;
    
    #endregion 
    
    #region Components
    Rigidbody _rigidbody;
    Collider _collider;
    #endregion
    
    #region Internal State

    private bool _canHurt = false;
    [CanBeNull] public GameObject owner;
    
    #endregion
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void TurnOffPhysics()
    {
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _collider.isTrigger = false;
    }

    public void TurnOnPhysics()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _collider.isTrigger = false;
    }

    public void SetCanHurt(bool canHurt) => _canHurt = canHurt;

    public void SetUpThrow()
    {
        TurnOnPhysics();
        SetCanHurt(true);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // If you can't hurt, do nothing
        if (!_canHurt)
            return;
        
        // If you can hurt, set this to not hurt anymore
        _canHurt = false;
        
        var health = collision.gameObject.GetComponent<Health>();
        if (!health)
            return;
        
        // TODO set perpetrator as player
        health.TakeDamage(damage, _rigidbody.linearVelocity.normalized, knockback, 3 * knockback, owner);
    }
}
