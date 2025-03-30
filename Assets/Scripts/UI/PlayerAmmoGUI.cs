using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerAmmoGUI : MonoBehaviour
{
    #region Inspector Properties
    [Description("Bullet icon prefab")]
    [SerializeField]
    private BulletIconAnimation bulletPrefab;
    
    [Description("Player related to this UI element")]
    [SerializeField]
    [CanBeNull] private Player player;
    
    #endregion
    
    
    #region Internal State

    [CanBeNull] private Gun _gun;
    List<BulletIconAnimation> bullets = new();

    #endregion

    private void Reset()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void OnEnable()
    {
        if (!player)
            player = FindFirstObjectByType<Player>();
    }
    
    private void Start()
    {
        // Init Health Value
        if (player)
        {
            SetupCallbacks();
            Init();
        }
    }

    private void OnDisable()
    {
        ClearCallbacks();
    }

    void UpdateAmmoCount()
    {
        if (!_gun)
        {
            ClearChildren();
            return;
        }
        
        if (_gun.CurrentAmmo > bullets.Count)
        {
            var diff = _gun.CurrentAmmo - bullets.Count;
            for (int i = 0; i < diff; i++)
            {
                CreateBullet();
            }
        }
        else
        {
            var diff = bullets.Count - _gun.CurrentAmmo;
            for (int i = 0; i < diff; i++)
            {
                DestroyBullet();
            }
        }
    }

    private void ClearChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void SetupCallbacks()
    {
        if (!player)
            return;
        
        player.Equipment.OnEquip += OnEquipmentChanged;
        player.Equipment.OnUnequip += OnUnequip;
        if (!player.Equipment.currentGun)
            return;
        
        player.Equipment.currentGun.OnShoot += OnWeaponAmmoChange;
    }

    void ClearCallbacks()
    {
        if (!player)
            return;
        
        player.Equipment.OnEquip -= OnEquipmentChanged;
        player.Equipment.OnUnequip -= OnUnequip;
    }

    private void Init()
    {
        if (player)
            _gun = player.Equipment.currentGun;

        foreach (Transform child in transform)
        {
            bullets.Add(child.GetComponent<BulletIconAnimation>());
        }
        UpdateAmmoCount();
    }

    private void OnEquipmentChanged(Equipable equipable)
    {
        // Clear previous gun callbacks
        if (_gun)
        {
            _gun.OnShoot -= OnWeaponAmmoChange;
        }
        
        // Set up the new gun callbacks
        _gun = equipable.GetComponent<Gun>();
        if (_gun)
            _gun.OnShoot += OnWeaponAmmoChange;
        
        UpdateAmmoCount();
    }

    private void OnUnequip(Equipable equipable)
    {
        ClearChildren();
        bullets.Clear();
    }

    private void OnWeaponAmmoChange(bool couldShoot)
    {
        if (couldShoot)
            UpdateAmmoCount();  
    }

    private void CreateBullet()
    {
        bullets.Add(Instantiate(bulletPrefab, transform));
    }

    private void DestroyBullet()
    {
        if (bullets.Count == 0)
            return;
        
        var bullet = bullets[0];
        bullets.RemoveAt(0);
        bullet.Kill();
    }
}
