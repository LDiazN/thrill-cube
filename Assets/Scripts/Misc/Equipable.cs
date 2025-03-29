using System;
using JetBrains.Annotations;
using UnityEngine;

public class Equipable : MonoBehaviour
{
    #region Components
    [CanBeNull] private Collider _collider;
    [CanBeNull] private Rigidbody _rigidbody;
    #endregion

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Turn on or off physics. On = true turns physics on, On = false turns physics off
    /// </summary>
    /// <param name="on"></param>
    public void TurnPhysics(bool on)
    {
        if (_collider)
            _collider.enabled = on;
        if (_rigidbody)
        {
            _rigidbody.isKinematic = !on;
            _rigidbody.useGravity = on;
        }
    }

    public void Equip(Transform parent)
    {
        TurnPhysics(false);
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Unequip()
    {
        TurnPhysics(true);
        transform.parent = null;
    }
}
