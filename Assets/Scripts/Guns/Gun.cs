using System;
using System.ComponentModel;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
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
    #endregion
    
    #region Callbacks

    public event Action OnShoot;
    
    #endregion 
    #region Internal State 
    private float _timeSinceLastShot;
    #endregion

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
    }

    public void Fire(Vector3 target)
    {
        if (_timeSinceLastShot < fireRate)
            return;
        
        _timeSinceLastShot = 0;
        SpawnBullet(target);
        ScreenShake();
        
        OnShoot?.Invoke();
    }

    private void SpawnBullet(Vector3 target)
    {
        var direction = target - bulletSpawnPoint.position;
        direction.Normalize();
        
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        
        SetupBullet(bullet, target);
    }

    private void ScreenShake()
    {
        if (Camera.main != null)
            _impulseSource.GenerateImpulse(Vector3.forward);        
    }

    private void SetupBullet(GameObject bullet, Vector3 target)
    {
        var bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.knockBackForce  = knockBackForce;
        bulletComponent.knockBackForceOnDead = knockBackForceOnDead;
        bullet.transform.LookAt(target);
    }
}
