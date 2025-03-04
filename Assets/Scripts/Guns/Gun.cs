using System.ComponentModel;
using UnityEngine;

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

    #region Internal State 
    private float timeSinceLastShot;
    #endregion

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void Fire(Vector3 target)
    {
        if (timeSinceLastShot < fireRate)
            return;
        
        timeSinceLastShot = 0;

        var direction = target - bulletSpawnPoint.position;
        direction.Normalize();
        
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.LookAt(target);
        
    }
}
