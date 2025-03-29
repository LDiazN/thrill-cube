using System.Collections;
using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource), typeof(SFXPlayer))]
public class Gun : MonoBehaviour
{
    #region Inspector Properties

    [Description("If this weapon should generate a weapon picker when stopped")] [SerializeField]
    private bool generatePicker = false;
    
    [Description("How many seconds to wait between shots")]
    [SerializeField]
    private float fireRate = 0.5f;
    public float FireRate => fireRate;
    
    [Description("Object to spawn when shooting this weapon")]
    [SerializeField]
    private GameObject bulletPrefab;
    
    [Description("Where to spawn the bullet")]
    [SerializeField]
    private Transform bulletSpawnPoint;

    [Description("Force to apply in the direction of the shot on contact")] [SerializeField]
    private float knockBackForce = 1;
    
    [Description("Force to apply in the direction of the shot when the enemy dies")] [SerializeField]
    private float knockBackForceOnDead = 20;

    [Description("Pickable to place in this gun's position when the gun is trown away")] [SerializeField]
    private WeaponPicker weaponPicker;

    [Header("Ammo")]
    [Description("How many bullets does this weapon have on start")] [SerializeField]
    private int magSize = 10;

    [Description("If this weapon has infinite ammo, mostly for enemies")] [SerializeField]
    private bool infiniteAmmo = false;

    [Description("Sound played when the player tries to shoot without ammo")] [SerializeField]
    private AudioClip noAmmoSound;
    #endregion

    #region Components 
    CinemachineImpulseSource _impulseSource;
    SFXPlayer _sfxPlayer;
    [CanBeNull] Equipable _equipable;
    [CanBeNull] Rigidbody _rigidbody;
    #endregion
    
    #region Callbacks

    // Called with true if could shoot, false otherwise
    public event Action<bool> OnShoot;
    
    #endregion 
    #region Internal State 
    private float _timeSinceLastShot;
    public bool IsOnRecoil => _timeSinceLastShot < fireRate;

    public int _currentAmmo;
    public int CurrentAmmo => _currentAmmo;

    private bool _canBecomePickable;
    #endregion

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _sfxPlayer = GetComponent<SFXPlayer>();
        _equipable = GetComponent<Equipable>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        OnShoot += PlayShootSound;
    }

    private void Start()
    {
        _currentAmmo = magSize;
    }

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
        UpdateEquipableGun();
    }

    /// <summary>
    /// Fire the gun at the specified target.
    /// </summary>
    /// <param name="target">Where to shoot at</param>
    /// <param name="owner">Owner of the bullet</param>
    public void Fire(Vector3 target, GameObject owner)
    {
        if (_timeSinceLastShot < fireRate)
            return;
        
        if (_currentAmmo == 0 && !infiniteAmmo)
        {
            OnShoot?.Invoke(false);
            return;
        }
        
        _timeSinceLastShot = 0;
        SpawnBullet(target, owner);
        ScreenShake();
        
        if (!infiniteAmmo)
            _currentAmmo = Mathf.Max(_currentAmmo - 1, 0);
        
        OnShoot?.Invoke(true);
    }

    private void SpawnBullet(Vector3 target, GameObject owner)
    {
        var direction = target - bulletSpawnPoint.position;
        direction.Normalize();
        
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        
        SetupBullet(bullet, target, owner);
    }

    private void ScreenShake()
    {
        if (Camera.main)
            _impulseSource.GenerateImpulse(Vector3.forward);        
    }

    private void SetupBullet(GameObject bullet, Vector3 target, GameObject owner)
    {
        var bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.knockBackForce  = knockBackForce;
        bulletComponent.knockBackForceOnDead = knockBackForceOnDead;
        bulletComponent.owner = owner;
        bullet.transform.LookAt(target);
    }

    private void PlayShootSound(bool couldShoot)
    {
        if (couldShoot)
        {
            _sfxPlayer.PlaySound();
            return;
        }

        _sfxPlayer.PlaySound(noAmmoSound);
    }

    public bool IsEmpty()
    {
        return _currentAmmo == 0;
    }

    private void CreateWeaponPicker()
    {
        var picker = Instantiate(weaponPicker);
        picker.transform.position = transform.position;
        picker.SetGun(this);
    }

    private void UpdateEquipableGun()
    {
        if (!_equipable || !_rigidbody || !generatePicker || IsEmpty() || !_canBecomePickable)
            return;

        if (_equipable.IsEquiped)
            return;
        
        // if not equiped and rigidbody says it's stopped, create a pickable object
        if (_rigidbody.linearVelocity.magnitude <= 0.01f)
        {
            CreateWeaponPicker();
        }
    }

    public void StartPickableTimer()
    {
        StartCoroutine(PickableTimer(1));
    }

    private IEnumerator PickableTimer(float time)
    {
        _canBecomePickable = false;
        yield return new WaitForSeconds(time);
        _canBecomePickable = true;
    }
}
