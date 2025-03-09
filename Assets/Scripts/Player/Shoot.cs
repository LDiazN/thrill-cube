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

        var gunComponent = Weapon.GetComponent<Gun>();
        Debug.Assert(gunComponent !=null, "A gun should always have a gun component");
        
        gunComponent.Fire(GetTarget(), gameObject);
    }

    private Vector3 GetTarget()
    {
        // Try to shoot at  target very far away. If you find something else first, shoot at that 
        var hitSomething = Physics.Raycast(_tpsCamera.GetCameraPosition(), _tpsCamera.GetCameraDirection(), out RaycastHit hit);
        if (hitSomething) 
            return hit.point;
        
        // Didn't find anything, shoot at the infinite void
        return _tpsCamera.GetCameraPosition() + 1000 * _tpsCamera.GetCameraDirection();
    }

    private void OnDrawGizmos()
    {
        if (_tpsCamera == null)
            return;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTarget(), 1);
    }
}
