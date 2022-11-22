using System;
using RSG;
using UnityEngine;

namespace _Scripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource SFXSource;

        [Header("Defaults"), SerializeField] private AudioClip successSFX;
        public event Action OnSoundEnded;
        public AudioSource MusicSource => musicSource;

        private void Awake()
        {
                AudioSessionSetter.SetAudioSession();
        }

        public IPromise PlayAudio(AudioClip clip, float startFrom = 0f, bool loop = false)
        {
            var clipPromise = new Promise();
            musicSource.clip = clip;
            musicSource.time = startFrom;
            musicSource.loop = loop;
            musicSource.Play();
            var timer = new PromiseTimer();
            timer.WaitFor(clip.length)
                .Then(() =>
                {
                    OnSoundEnded?.Invoke();
                    clipPromise.ResolveIfPending();
                }).Catch(Debug.LogException); 
            return clipPromise;
        }

        public void PauseAudio()
        {
            musicSource.Pause();
            musicSource.loop = false;
        }

        public void PlaySFX(AudioClip clip = null)
        {
            if (clip == null)
                clip = successSFX;
            SFXSource.PlayOneShot(clip);
        }
    }
}