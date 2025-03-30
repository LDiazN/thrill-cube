using System;
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

    private void OnEnable()
    {
        _gun.OnShoot += TriggerShootingAnimation;
    }

    private void OnDisable()
    {
        _gun.OnShoot -= TriggerShootingAnimation;
    }

    private void TriggerShootingAnimation(bool couldShoot)
    {
        _animator.speed = 1f / _gun.FireRate;
        if (couldShoot)
        {
            _animator.SetTrigger(GunAnimationParams.shoot);
        }
        _animator.SetTrigger(GunAnimationParams.failedShot);
    }
}
