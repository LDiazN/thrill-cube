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
    
    #endregion

    #region Components 
    CinemachineImpulseSource _impulseSource;
    SFXPlayer _sfxPlayer;
    #endregion
    
    #region Callbacks

    public event Action OnShoot;
    
    #endregion 
    #region Internal State 
    private float _timeSinceLastShot;
    public bool IsOnRecoil => _timeSinceLastShot < fireRate;
    #endregion

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _sfxPlayer = GetComponent<SFXPlayer>();
    }

    private void OnEnable()
    {
        OnShoot += _sfxPlayer.PlaySound;
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
        
        _timeSinceLastShot = 0;
        SpawnBullet(target, owner);
        ScreenShake();
        
        OnShoot?.Invoke();
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
    
}
