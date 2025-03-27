using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonCamera), typeof(Equipment))]
public class Shoot : MonoBehaviour
{
    #region Internal State
    private ThirdPersonCamera _tpsCamera;
    private Equipment _equipment;
    #endregion

    private void Awake()
    {
        _tpsCamera = GetComponent<ThirdPersonCamera>();
        _equipment = GetComponent<Equipment>();
    }

    public void Fire()
    {
        if (!_equipment.currentGun)
            return;

        var gun = _equipment.currentGun;
        Debug.Assert(gun, "A gun should always have a gun component");
        
        gun.Fire(_tpsCamera.GetTarget(), gameObject);
    }

    private void OnDrawGizmos()
    {
        if (_tpsCamera == null)
            return;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_tpsCamera.GetTarget(), 1);
    }
}
