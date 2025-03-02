using System;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonCamera))]
public class Shoot : MonoBehaviour
{
    #region Inspector Variables
    
    // Might be null if no weapon is attached
    public GameObject Weapon;
    
    #endregion
    
    #region Internal State
    private ThirdPersonCamera _tpsCamera;
    #endregion

    private void Awake()
    {
        _tpsCamera = GetComponent<ThirdPersonCamera>();
    }

    public void Fire()
    {
        if (Weapon == null)
            return;

        Debug.Log("Bang bang!");
        var gunComponent = Weapon.GetComponent<Gun>();
        Debug.Assert(gunComponent !=null, "A gun should always have a gun component");
        
        gunComponent.Fire(GetTarget());
    }

    private Vector3 GetTarget()
    {
        return _tpsCamera.GetCameraPosition() + 1000 * _tpsCamera.GetCameraDirection();
    }

}
