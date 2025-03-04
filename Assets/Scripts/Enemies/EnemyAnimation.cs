using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Health))]
public class EnemyAnimation : MonoBehaviour
{
    #region Components
    Animator _animator;
    Health _health;
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
    }
}
