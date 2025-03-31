using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class AudioStarter : MonoBehaviour
{
    #region Inspector Properties
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float timeToFade = 5;
    #endregion

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        var originalVolume = audioSource.volume;
        audioSource.volume = 0;
        StartCoroutine(AudioFade(originalVolume));
    }

    private IEnumerator AudioFade(float targetVolume = 1)
    {
        var startingTime = 0f;

        while (startingTime < timeToFade)
        {
            startingTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, targetVolume, startingTime / timeToFade);
            yield return new WaitForNextFrameUnit();
        }
        audioSource.volume = targetVolume;
    }
}
