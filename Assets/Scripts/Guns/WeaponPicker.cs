using System;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class WeaponPicker : MonoBehaviour
{
    #region Inspector Properties

    [Description("Weapon that the player will receive after picking this picker")] [SerializeField]
    public Equipable gun;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        
        // TODO Hacer esto desde el manejador de equipamiento
        // var playerHasGun = player.Equipment.currentGun != null;
        // var gunHasAmmo = playerHasGun && !player.Equipment.currentGun.IsEmpty();
        // if (playerHasGun && gunHasAmmo)
        //     return;

        player.Equipment.CanPick = this;
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        
        if (player.Equipment.CanPick == this)
            player.Equipment.CanPick = null;
    }

    /// <summary>
    /// Generate the weapon related to this object.
    /// You should destroy the picker after getting this weapon
    /// </summary>
    /// <returns></returns>
    public Equipable RetrieveWeapon(bool destroyAfterGenerate = false)
    {
        var gunInScene = gun.gameObject.scene.IsValid();
        var obj = gunInScene ? gun: Instantiate(gun);

        if (gunInScene)
            obj.gameObject.SetActive(true);

        if (destroyAfterGenerate)
        {
            Destroy(gameObject, 0.01f);
        }

        return obj;
    }

    /// <summary>
    /// Set the gun for this equipable. If prefab, just changes the template prefab.
    /// otherwise deactivates the object and stores it for later
    /// </summary>
    /// <param name="gun">Gun to equip</param>
    public void SetGun(Gun gun)
    {
        var equipable = gun.GetComponent<Equipable>();
        this.gun = equipable;
        var gunInScene = gun.gameObject.scene.IsValid();
        if (gunInScene)
            gun.gameObject.SetActive(false);
    }
}
