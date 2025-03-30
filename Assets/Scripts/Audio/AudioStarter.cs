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

    private void Start()
    {
        audioSource.volume = 0;
        StartCoroutine(AudioFade());
    }

    private IEnumerator AudioFade()
    {
        var startingTime = 0f;

        while (startingTime < timeToFade)
        {
            startingTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 1, startingTime / timeToFade);
            yield return new WaitForNextFrameUnit();
        }
        audioSource.volume = 1;
    }
}
