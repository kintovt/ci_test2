using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MusicController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] BattleClips;
    public AudioClip MainMenuClip;

    public void StartRandomBattleClip(float fadeDuration)
    {
        audioSource.clip = BattleClips[Random.Range(0, BattleClips.Length)];
        audioSource.Play();
        audioSource.DOFade(1, fadeDuration);
    }

    public void StartMainMenuClip(float fadeDuration)
    {
        audioSource.clip = MainMenuClip;
        audioSource.Play();
        audioSource.DOFade(1, fadeDuration);
    }

    public void StopMusic(float fadeDuration)
    {
        audioSource.DOFade(0, fadeDuration).onComplete = () => audioSource.Stop();
    }
}
