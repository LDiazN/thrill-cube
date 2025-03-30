using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BulletIconAnimation : MonoBehaviour
{
    #region Components
    Animator animator;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void Kill()
    {
        animator.SetTrigger(UIAnimationParams.fly);
    }
}
