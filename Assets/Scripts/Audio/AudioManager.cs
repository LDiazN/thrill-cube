using System.ComponentModel;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Inspector properties
    
    [SerializeField] private AudioSource playerAudioSource;
    [Description("Used to spawn an audio source at a specified position, mostly for SFX")]
    [SerializeField] private AudioSource audioSourcePrefab;
    
    #endregion
    
    #region Internal State 
    public static AudioManager Instance;
    #endregion

    private void Reset()
    {
        var player = FindFirstObjectByType<Player>();
        playerAudioSource = player.PlayerAudioSource;
    }

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
        
        if (!audioSourcePrefab)
            Debug.LogWarning("No audio source prefab set up in inspector!");
    }

    /// <summary>
    /// Play audio at the player's main audio source, mostly used for music
    /// </summary>
    /// <param name="clip">Clip to play</param>
    /// <param name="volume">Volume value, capped to [0,1]</param>
    public static void PlayAudio(AudioClip clip, float volume = 1)
    {
        if (!Instance)
            return;
        
        Instance.playerAudioSource.clip = clip;
        Instance.playerAudioSource.volume = Mathf.Clamp(volume, 0, 1);
        Instance.playerAudioSource.Play();
    }

    /// <summary>
    /// Play audio at a specified position, mostly used for SFX
    /// </summary>
    /// <param name="position">Where to place this audio source</param>
    /// <param name="clips">Clips to choice from</param>
    /// <param name="volume">Volume value, capped to [0,1]</param>
    /// <param name="randomizePitch">If should randomize pitch of this audio</param>
    public static void PlayAudioAtPosition(Vector3 position, AudioClip[] clips, float volume = 1,
        bool randomizePitch = true)
    {
        if (!Instance)
            return;
        
        Instance.MPlayAudioAtPosition(position, clips, volume, randomizePitch);
    }
    
    private void MPlayAudioAtPosition(Vector3 position, AudioClip[] clips, float volume = 1, bool randomizePitch = true)
    {
        var audioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity);
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.volume = Mathf.Clamp(volume, 0, 1);
        audioSource.Play();
        if (randomizePitch)
            audioSource.pitch = Random.Range(0.9f, 1.1f);
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
