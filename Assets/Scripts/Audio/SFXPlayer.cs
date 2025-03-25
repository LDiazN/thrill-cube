using System;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    #region Inspector Properties

    public string SFXPlayerName;
    [SerializeField] private AudioClip[] clips;
    [Description("Audio source used for this SFX player. Defaults to whatever audio source is attached to this gameobject if not provided")]
    [SerializeField] private AudioSource audioSource;
    
    #endregion

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        PlaySound(clips[Random.Range(0, clips.Length)]);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.time = Random.Range(0, clips.Length / 2f);
        audioSource.PlayOneShot(clip);
    }
    
}
