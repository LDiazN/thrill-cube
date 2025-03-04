using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Bullet : MonoBehaviour
{
    #region Inspector Variables

    [Description("How fast this bullet flies")] [SerializeField]
    private float maxSpeed = 10f;

    [Description("Particles to spawn on dead")] [SerializeField]
    private GameObject destroyParticles;

    #endregion
    
    #region Inspector Variables
    Rigidbody _rigidbody;
    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
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

        transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy(gameObject)).Play();
    }
}