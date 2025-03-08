using UnityEngine;

[RequireComponent(typeof(Gun), typeof(Animator))]
public class GunAnimation : MonoBehaviour
{
    #region Components
    Animator _animator;
    Gun _gun;
    
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _gun = GetComponent<Gun>();
    }

    private void Start()
    {
        _gun.OnShoot += (TriggerShootingAnimation);
    }

    private void TriggerShootingAnimation()
    {
        _animator.speed = 1f / _gun.FireRate;
        _animator.SetTrigger(GunAnimationParams.shoot);
    }
}
