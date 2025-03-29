using System;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// This is a helper class to ease access to several player elements, like health or movement
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Health), typeof(Shoot))]
public class Player : MonoBehaviour
{
    #region Inspector properties
    [Header("Audio")]
    [Description("Played when hurt")]
    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioSource playerAudioSource;
    public AudioSource PlayerAudioSource => playerAudioSource;

    [Description("Used for SFX inside the player")]
    [SerializeField] private AudioSource playerSFX;
    public AudioSource PlayerSFX => playerSFX;
    #endregion
    
    #region Components
    
    Health _health;
    public Health Health => _health;
    
    Shoot _shoot;
    public Shoot Shoot => _shoot;

    Throw _throw;
    public Throw Throw => _throw;
    
    Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;
    
    PlayerMovement _playerMovement;
    public PlayerMovement PlayerMovement => _playerMovement;
    
    Equipment _equipment;
    public Equipment Equipment => _equipment;
    
    ThirdPersonCamera _tpsCamera;
    public ThirdPersonCamera TPSCamera => _tpsCamera;

    #endregion

    private void Awake()
    {
        _health = GetComponent<Health>();
        _shoot = GetComponent<Shoot>();
        _throw = GetComponent<Throw>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerMovement = GetComponent<PlayerMovement>();
        _equipment = GetComponent<Equipment>();
        _tpsCamera = GetComponent<ThirdPersonCamera>();
        
        if(!playerAudioSource)
            Debug.LogWarning("No player audio source set up in inspector!");
    }

    private void OnEnable()
    {
        _health.OnHealthChanged += HealthChanged;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HealthChanged;
    }

    private void Start()
    {
    }

    // Deactivates player input, doesn't matter if it's AI or human
    public void DeactivateInput()
    {
        var controller = GetComponent<PlayerController>();
        controller.enabled = false;
    }

    private void HealthChanged(Health health, Health.Change change)
    {
        if (change.IsDamage) 
            AudioManager.PlayAudioAtPosition(transform.position, hurtClips);
    }
}
