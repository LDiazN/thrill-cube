using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Manages what the player has in its hands right now
/// </summary>
public class Equipment : MonoBehaviour
{
    #region Inspector Properties

    [Description("Where equipables are attached in the player")] [SerializeField]
    private Transform equipmentSocket;
    
    [Description("Currently equiped object")]
    [SerializeField] private GameObject currentlyEquippedObject;

    [Description("SFX player used for playing gun pick SFX")]
    [CanBeNull] [SerializeField] private SFXPlayer gunPickSFX;
    
    [Description("Throw force when unequiping this item and launching it into the air")]
    [SerializeField] private float throwForce;

    #endregion

    #region Components

    // In case this character can shoot
    [CanBeNull] private Shoot _shoot;
    
    // In case this character can throw
    [CanBeNull] private Throw _throw;

    #endregion

    #region Internal State

    [CanBeNull] public Gun currentGun;
    [CanBeNull] public Throwable currenThrowable;
    
    #endregion
    
    private void Awake()
    {
        SetupEquipment();
    }
    
    [ContextMenu("Set up equipment")]
    public void SetUpEquipmentInEditor()
    {
        _throw = GetComponent<Throw>();
        _shoot = GetComponent<Shoot>();
        
        if (equipmentSocket.transform.childCount == 0)
            return;

        var possibleEquipment = equipmentSocket.transform.GetChild(0);
        currentlyEquippedObject = possibleEquipment.gameObject;
        
        SetupEquipment();
    }

    public void Equip([CanBeNull] GameObject equipable)
    {
        // Unequip whatever you have
        Unequip();
        
        // equipable can be either prefab, gameobject, or null
        GameObject toEquip = equipable && equipable.scene.IsValid() ? equipable : Instantiate(equipable); ;

        if (toEquip)
        {
            toEquip.transform.SetParent(equipmentSocket.transform);
            toEquip.transform.localPosition = Vector3.zero;
            currentGun = toEquip.GetComponent<Gun>();
        
            // If not a gun or the character can't shoot, add it as a throwable
            currenThrowable = toEquip.GetComponent<Throwable>();
            if (currenThrowable&& _throw)
            {
                _throw.SetUpThrowable(currenThrowable);                        
            }
            
            // If it does have a rigidbody, neutralize it
            var equipRb = toEquip.GetComponent<Rigidbody>();
            var equipCol = toEquip.GetComponent<Collider>();
            if (equipRb)
            {
                equipRb.isKinematic = true;
                equipRb.useGravity = false;
            }
            if (equipRb)
                equipCol.enabled = false;
            
            SetupEquipment();
        }
        
        currentlyEquippedObject = toEquip;
            
        if (equipable)
            gunPickSFX?.PlaySound();
    }

    public void Unequip(bool shouldThrow = true)
    {
        if (!currentlyEquippedObject)
            return;
        
        var oldObject = currentlyEquippedObject;
        oldObject.transform.SetParent(null);
        var objRigidbody = oldObject.GetComponent<Rigidbody>();
        if (objRigidbody && shouldThrow)
            TryThrowObject(objRigidbody);

        currentlyEquippedObject = null;
        currentGun = null;
        currenThrowable = null;
    }

    public void SetupEquipment()
    {
        _throw = GetComponent<Throw>();
        _shoot = GetComponent<Shoot>();
        if (currentlyEquippedObject)
        {
            currentGun = currentlyEquippedObject.GetComponent<Gun>();
            currenThrowable = currentlyEquippedObject.GetComponent<Throwable>();
        }
    }

    private void TryThrowObject(Rigidbody objRb)
    {
        // Throw object into the air if possible
        objRb.isKinematic = false;
        var direction = transform.up + transform.right;
        direction.Normalize();
        objRb.AddForce(direction * throwForce);
    }
}
