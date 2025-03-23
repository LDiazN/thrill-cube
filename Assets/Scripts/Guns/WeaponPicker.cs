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
        if (!player || !(player.Shoot.Weapon && player.Shoot.Weapon.IsEmpty()))
            return;

        player.Shoot.SetWeapon(gun);
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player exited!");
    }
}
