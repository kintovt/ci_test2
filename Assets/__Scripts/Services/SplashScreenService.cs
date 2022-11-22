using System;
using System.Threading.Tasks;
using DG.Tweening;
using RSG;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts
{
    public class SplashScreenService : MonoBehaviour
    {
        [Inject] private ITimeService timeService;
        
        [SerializeField] private GameObject logo;
        [SerializeField] private GameObject bg;
        [SerializeField] private float duration;
        [SerializeField] private AnimationCurve logoDisappear;
        [SerializeField] private Slider progressBar;

        private Promise currentPromise;
        private bool active;
        
        [Button]
        public IPromise Show()
        {
            Debug.Log("SplashScreenService.Show()");
            if (active) return Promise.Resolved();
            gameObject.SetActive(true);
            bg.transform.localScale = Vector3.zero;
            logo.transform.localScale = Vector3.zero;
            logo.SetActive(false);
            bg.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(bg.transform.DOScale(Vector3.one * 4f, duration/3));
            sequence.AppendCallback(() => logo.SetActive(true));
            sequence.Insert(duration / 4, logo.transform.DOScale(Vector3.one, duration / 2).SetEase(Ease.OutBounce));
            sequence.Play();
            currentPromise = timeService.WaitForTime(duration);
            active = true;
            return currentPromise;
        }
        
        [Button]
        public IPromise Hide()
        {
            Debug.Log("SplashScreenService.Hide()");
            bg.transform.localScale = Vector3.one * 4f;
            logo.SetActive(true);
            bg.SetActive(true);
            var sequence = DOTween.Sequence();
            sequence.Append(logo.transform.DOScale(Vector3.zero, duration / 3).SetEase(logoDisappear));
            sequence.Insert(duration / 4f, bg.transform.DOScale(Vector3.zero, duration/2));
            sequence.Play();
            currentPromise = timeService.WaitForTime(duration);
            currentPromise.Then(() =>
            {
                gameObject.SetActive(false);
                active = false;
            }).Catch(Debug.LogException);
            return currentPromise;
        }

        public Slider GetProgressBar()
        {
            return progressBar;
        }
    }
}