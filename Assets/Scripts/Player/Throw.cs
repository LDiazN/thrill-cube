using System;
using System.ComponentModel;
using UnityEngine;

public class Throw : MonoBehaviour
{
    #region Inspector Variables
    
    [Description("Game object where the throwable will be attached after changing weapons")] [SerializeField]
    private Transform equipmentSlot;

    [Description("How strong to throw the throwable away when unequiping it")] [SerializeField]
    private float unequipForce = 20;
    
    [Description("How strong to throw the throwable away when actually throwing it")] [SerializeField]
    private float throwForce = 100;
    
    #endregion
    
    #region Internal State
    // Can be either gameobject or prefab
    Throwable _throwable = null;
    #endregion
    
    #region Callbacks

    public event Action OnThrow;
    
    #endregion

    public void SetThrowable(Throwable throwable)
    {
        if (_throwable == throwable) // Nothing to do
            return; 
        
        if (!_throwable)
            Unequip();            
        
        _throwable = throwable;
        _throwable.transform.SetParent(equipmentSlot.transform);
        _throwable.transform.localPosition = Vector3.zero;
        
        var throwableComponent = _throwable.GetComponent<Throwable>();
        if (throwableComponent)
        {
            throwableComponent.owner = gameObject;
            throwableComponent.TurnOffPhysics();
        }
    }

    public void ThrowEquipment(Vector3 direction)
    {
        _throwable.transform.parent = null;
        var throwableComponent = _throwable.GetComponent<Throwable>();
        var rigidBody = throwableComponent.GetComponent<Rigidbody>();
        
        throwableComponent.SetUpThrow();
        
        _throwable.transform.SetParent(null);
        _throwable.transform.position += transform.forward;
        
        rigidBody.AddForce(direction * throwForce, ForceMode.Impulse);
        
        OnThrow?.Invoke();
    }

    private void Unequip()
    {
        _throwable.transform.parent = null;
        
        // Launch it into the air if has rigidbody
        var rigidBody = _throwable.GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
        var direction = Vector3.up + Vector3.right;
        direction.Normalize();
        
        rigidBody.AddForce(direction * unequipForce, ForceMode.Impulse);
    }
}
