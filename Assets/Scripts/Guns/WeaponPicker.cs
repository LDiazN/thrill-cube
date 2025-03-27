using System;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class WeaponPicker : MonoBehaviour
{
    #region Inspector Properties

    [Description("Weapon that the player will receive after picking this picker")] [SerializeField]
    private Gun gun;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player)
            return;
        
        var playerHasGun = player.Equipment.currentGun != null;
        var gunHasAmmo = playerHasGun && !player.Equipment.currentGun.IsEmpty();
        if (playerHasGun && gunHasAmmo)
            return;
        
        player.Equipment.Equip(gun.gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player exited!");
    }
}
