using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Inspector Properties

    [Description("How fast this bullet flies")] [SerializeField]
    private float maxSpeed = 10f;

    [Description("Particles to spawn on dead")] [SerializeField]
    private GameObject destroyParticles;

    [Description("How much damage should this bullet do")] 
    public int damage = 1;
    
    [Header("Audio")]
    [Description("Played when hitting something not alive")]
    public AudioClip[] wallHitClips;
    #endregion
    
    #region Components
    Rigidbody _rigidbody;
    Collider _collider;
    #endregion
    
    #region Internal State

    [HideInInspector]
    public float knockBackForce = 1;
    
    [HideInInspector]
    public float knockBackForceOnDead = 20;

    [HideInInspector] public GameObject owner;

    #endregion 

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        var translation = maxSpeed * Time.deltaTime * transform.forward;
        transform.Translate(translation, Space.World);
    }

    private void OnCollisionEnter(Collision other)
    {
        _rigidbody.isKinematic = true;
        if (destroyParticles)
            Instantiate(destroyParticles, transform.position, Quaternion.identity);

        // Check if you can hurt the other entity
        var health = other.gameObject.GetComponent<Health>();
        if (health)
            health.TakeDamage(damage, transform.forward, knockBackForce, knockBackForceOnDead, owner);
        else if (wallHitClips.Length > 0)
            AudioManager.PlayAudioAtPosition(transform.position, wallHitClips);
        
        // Start animation and destroy this bullet
        transform.DOScale(Vector3.zero, 0.1f).OnComplete(() => Destroy(gameObject)).Play();
        
        // Deactivate collisions to prevent phantom hits
        _collider.enabled = false;
    }
}