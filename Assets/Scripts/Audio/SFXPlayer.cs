using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] private AudioClip[] clips;
    
    #endregion
    #region Components
    private AudioSource _audioSource;
    #endregion

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.time = Random.Range(0, clips.Length / 2f);
        _audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
    
}
