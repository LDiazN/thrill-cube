using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemySFXPlayer : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioClip[] deathClips;
    
    #endregion
    
    #region Components

    private Enemy _enemy;

    #endregion

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        _enemy.Health.OnHealthChanged += PlayHurtSFX;
    }

    private void PlayHurtSFX(Health health, Health.Change change)
    {
        AudioClip[] clips = {null};
        var index = Mathf.Min(health.CurrentHealth, hurtClips.Length - 1);
        // Invert it 
        index = (hurtClips.Length - 1 - index);
        clips[0] = hurtClips[index];
        if (change.IsDamage && !change.JustDied(health))
            AudioManager.PlayAudioAtPosition(transform.position, clips);
        else if (change.IsDamage)
            AudioManager.PlayAudioAtPosition(transform.position, deathClips);
    }
}
