using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonCamera))]
public class Throw : MonoBehaviour
{
    #region Inspector Variables
    
    [Description("Game object where the throwable will be attached after changing weapons")] [SerializeField]
    private Transform equipmentSlot;

    [Description("How strong to throw the throwable away when actually throwing it")] [SerializeField]
    private float throwForce = 100;
    
    #endregion
    
    #region Components
    
    ThirdPersonCamera _tpsCamera;
    
    Equipment _equipment;
    
    #endregion
    
    
    #region Callbacks

    public event Action OnThrow;
    
    #endregion

    private void Awake()
    {
        _tpsCamera = GetComponent<ThirdPersonCamera>();
        _equipment = GetComponent<Equipment>();
    }

    public void SetUpThrowable(Throwable throwable)
    {
        if (throwable)
        {
            throwable.owner = gameObject;
            throwable.TurnOffPhysics();
        }
    }

    public void ThrowEquipment()
    {
        ThrowEquipment(_tpsCamera.GetTarget());
    }
    
    public void ThrowEquipment(Vector3 target)
    {
        if (!_equipment.currenThrowable)
            return;
        
        var direction = target - transform.position;
        direction.Normalize();

        var throwable = _equipment.currenThrowable;
        var throwableComponent = throwable.GetComponent<Throwable>();
        var rigidBody = throwableComponent.GetComponent<Rigidbody>();
        
        throwableComponent.SetUpThrow();
        
        throwable.transform.SetParent(null);
        throwable.transform.position += transform.forward;
        
        rigidBody.AddForce(direction * throwForce, ForceMode.Impulse);
        
        _equipment.Unequip(false);
        
        OnThrow?.Invoke();
    }
}
