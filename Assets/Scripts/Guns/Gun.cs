using System;
using System.ComponentModel;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource), typeof(SFXPlayer))]
public class Gun : MonoBehaviour
{
    #region Inspector Properties
    
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
    #endregion

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _sfxPlayer = GetComponent<SFXPlayer>();
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
}
