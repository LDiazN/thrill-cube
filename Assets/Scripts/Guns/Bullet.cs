using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Bullet : MonoBehaviour
{
    #region Inspector Properties

    [Description("How fast this bullet flies")] [SerializeField]
    private float maxSpeed = 10f;

    [Description("Particles to spawn on dead")] [SerializeField]
    private GameObject destroyParticles;

    [Description("How much damage should this bullet do")] 
    public int damage = 1;
    
    #endregion
    
    #region Inspector Variables
    Rigidbody _rigidbody;
    Collider _collider;
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
        if (health != null)
            health.TakeDamage(damage, transform.forward);
        
        // Start animation and destroy this bullet
        transform.DOScale(Vector3.zero, 0.1f).OnComplete(() => Destroy(gameObject)).Play();
        
        // Deactivate collisions to prevent phantom hits
        _collider.enabled = false;
    }
}