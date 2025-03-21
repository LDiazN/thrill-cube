using System.Collections;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// This class implements knockback on hurt
/// </summary>
[RequireComponent(typeof(Health), typeof(Rigidbody))]
public class Knockback : MonoBehaviour
{
    #region Inspector Variables

    [Description("Multiplier to make knockback stronger or weaker")]
    [SerializeField] float knockbackMultiplier = 1;
    
    [Description("Time that the player won't be able to move after a hit")]
    [SerializeField] float knockbackTime = 0.5f;
    
    #endregion
    
    #region Components
    Health _health;
    Rigidbody _rigidbody;
    // Optional, only present if attached to player
    [CanBeNull] Player _player;
    #endregion

    private void Awake()
    {
        _health = GetComponent<Health>();
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += OnHurt;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= OnHurt;
    }

    private void OnHurt(Health health, Health.Change change)
    {
        if (!change.IsDamage)
            return;
        
        var force = change.JustDied(health) ? change.knockbackOnDead : change.knockback;
        force *= knockbackMultiplier;
        
        Knock(force, change.direction);
        
        if (_player)
            OnHurtPlayer(change);
    }

    public void Knock(float force, Vector3 direction)
    {
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);        
    }

    private void OnHurtPlayer(Health.Change change)
    {
        StartCoroutine(EnablePlayerMovement());
    }

    private IEnumerator EnablePlayerMovement()
    {
        _player.PlayerMovement.CanMove = false;
        yield return new WaitForSeconds(knockbackTime);
        _player.PlayerMovement.CanMove = true;
    }
}
