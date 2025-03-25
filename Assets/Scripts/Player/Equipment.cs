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

    [Header("Debug")] 
    [Description("A prefab used to equip a throwable in debug mode")]
    [SerializeField] private Throwable exampleThrowable;

    #endregion

    #region Components

    // In case this character can shoot
    [CanBeNull] private Shoot _shoot;
    
    // In case this character can throw
    [CanBeNull] private Throw _throw;

    #endregion

    private void Awake()
    {
        SetUpEquipment();
    }
    
    [ContextMenu("Set up equipment")]
    public void SetUpEquipment()
    {
        _shoot = GetComponent<Shoot>();
        _throw = GetComponent<Throw>();
        
        var gun = currentlyEquippedObject.GetComponent<Gun>();
        if (gun && _shoot)
            _shoot.SetWeapon(gun);
        
        var throwable = currentlyEquippedObject.GetComponent<Throwable>();
        if (throwable && _throw)
            _throw.SetThrowable(throwable);
    }

    private void Equip(GameObject equipable)
    {
        // If gun, defer this work to the shoot component
        var gun = equipable.GetComponent<Gun>();

        if (gun && _shoot)
        {
            _shoot.SetWeapon(gun);
        }
        
        // If not a gun or the character can't shoot, add it as a throwable
        var throwable = equipable.GetComponent<Throwable>();
        if (throwable && _throw)
        {
            _throw.SetThrowable(throwable);                        
        }
    }
}
