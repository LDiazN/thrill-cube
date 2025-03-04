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
    
    [Description("Object to spawn when shooting this weapon")]
    [SerializeField]
    private GameObject bulletPrefab;
    
    [Description("Where to spawn the bullet")]
    [SerializeField]
    private Transform bulletSpawnPoint;
    #endregion

    #region Components 
    CinemachineImpulseSource _impulseSource;
    #endregion
    
    #region Internal State 
    private float timeSinceLastShot;
    #endregion

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void Fire(Vector3 target)
    {
        if (timeSinceLastShot < fireRate)
            return;
        
        timeSinceLastShot = 0;
        SpawnBullet(target);
        ScreenShake();
    }

    private void SpawnBullet(Vector3 target)
    {
        var direction = target - bulletSpawnPoint.position;
        direction.Normalize();
        
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.LookAt(target);
    }

    private void ScreenShake()
    {
        if (Camera.main != null)
            _impulseSource.GenerateImpulse(Vector3.forward);        
    }
}
