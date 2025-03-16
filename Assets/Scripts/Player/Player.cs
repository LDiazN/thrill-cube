using System;
using UnityEngine;

/// <summary>
/// This is a helper class to ease access to several player elements, like health or movement
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Health), typeof(Shoot))]
public class Player : MonoBehaviour
{
    #region Components
    
    Health _health;
    public Health Health => _health;
    
    Shoot _shoot;
    public Shoot Shoot => _shoot;
    
    Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;
    
    PlayerMovement _playerMovement;
    public PlayerMovement PlayerMovement => _playerMovement;

    #endregion

    private void Awake()
    {
        _health = GetComponent<Health>();
        _shoot = GetComponent<Shoot>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        ConnectToGUI();
        _health.OnHealthChanged += HealthChanged;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HealthChanged;
    }

    // Deactivates player input, doesn't matter if it's AI or human
    public void DeactivateInput()
    {
        var controller = GetComponent<PlayerController>();
        controller.enabled = false;
    }

    private void HealthChanged(Health health, Health.Change change)
    {
        
    }

    private void ConnectToGUI()
    {
        var playerUI = FindFirstObjectByType<PlayerHealthGUI>();
        playerUI.Player = this;
    }
}
