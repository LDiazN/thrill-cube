using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonCamera))]
public class Shoot : MonoBehaviour
{
    #region Inspector Variables
    
    // Might be null if no weapon is attached
    [CanBeNull] public Gun Weapon;
    
    
    [Description("Game object where the gun will be attached after changing weapons")] [SerializeField]
    private Transform weaponSlot;
    
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

    public void SetWeapon(Gun gunPrefab)
    {
        var oldWeapon = Weapon;
        Weapon = Instantiate(gunPrefab, weaponSlot);
        System.Diagnostics.Debug.Assert(Weapon != null, nameof(Weapon) + " != null");
        Weapon.transform.localPosition = Vector3.zero;
        Weapon.transform.localRotation = Quaternion.identity;
        if (oldWeapon)
            Destroy(oldWeapon.gameObject);
    }
}
