using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Manages what the player has in its hands right now
/// </summary>
public class Equipment : MonoBehaviour
{
    #region Inspector Properties

    [Description("Where equipables are attached in the player")] [SerializeField]
    private Transform equipmentSocket;
    
    [Description("Currently equiped object")]
    [SerializeField] private Equipable currentlyEquippedObject;

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
    /// <summary>
    /// An object that can be picked by the player if he wants to, normally part of a pickable
    /// </summary>
    [FormerlySerializedAs("CanPick")]
    [HideInInspector]
    [CanBeNull] public WeaponPicker canPickFromPicker;
    
    /// <summary>
    /// An object in the floor that could be picked by the character
    /// </summary>
    [HideInInspector]
    [CanBeNull] public Equipable canPickFromFloor;
    
    #endregion
    
    #region Callbacks

    public Action<Equipable> OnEquip;
    
    #endregion
    
    private void Awake()
    {
        SetupEquipment();
    }

    private void Update()
    {
        UpdateQuickChange();
    }

    [ContextMenu("Set up equipment")]
    public void SetUpEquipmentInEditor()
    {
        _throw = GetComponent<Throw>();
        _shoot = GetComponent<Shoot>();
        
        if (equipmentSocket.transform.childCount == 0)
            return;

        var possibleEquipment = equipmentSocket.transform.GetChild(0);
        currentlyEquippedObject = possibleEquipment.gameObject.GetComponent<Equipable>();
        
        SetupEquipment();
    }

    public void Equip([CanBeNull] GameObject equipable)
    {
        if (equipable)
        {
            var equipComponent = equipable.GetComponent<Equipable>();
            Equip(equipComponent);
        }
        else 
            Equip((Equipable) null);
    }
    
    public void Equip([CanBeNull] Equipable equipable)
    {
        // Unequip whatever you have
        Unequip();
        
        // equipable can be either prefab, gameobject, or null
        Equipable toEquip = equipable && equipable.gameObject.scene.IsValid() ? equipable : Instantiate(equipable); ;

        if (toEquip)
        {
            toEquip.Equip(equipmentSocket);
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
        
        OnEquip?.Invoke(toEquip);
    }

    public void Unequip(bool shouldThrow = true)
    {
        if (!currentlyEquippedObject)
            return;
        
        var oldObject = currentlyEquippedObject;
        oldObject.Unequip();
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
        var gun = objRb.GetComponent<Gun>();
        gun?.StartPickableTimer();
        
        // Throw object into the air if possible
        var direction = transform.up + transform.right;
        direction.Normalize();
        objRb.AddForce(direction * throwForce, ForceMode.Impulse);
    }

    public bool TryEquipFromPicker()
    {
        if (!canPickFromPicker)
            return false;
        
        Unequip();
        Equip(canPickFromPicker.RetrieveWeapon(true));
        canPickFromPicker = null;
        return true;
    }

    public bool TryEquipFromFloor()
    {
        if (!canPickFromFloor) return false;
        
        Unequip();
        Equip(canPickFromFloor);
        canPickFromFloor = null;

        return true;
    }

    public void UpdateQuickChange()
    {
        if ((currentGun && currentGun.IsEmpty()) || !currentlyEquippedObject)
        {
            TryEquipFromPicker();
        }
            
    }
}
