using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DanceElementsResultView : MonoBehaviour
    {
        [SerializeField] private TMP_Text percentsText;
        [SerializeField] private TMP_Text elementText;
        [SerializeField] private float popDuration = 1f; 

        private CanvasGroup canvasGroup;

        public CanvasGroup CanvasGroup
        {
            get
            {
                canvasGroup ??= GetComponent<CanvasGroup>();
                return canvasGroup;
            }
        }

        public float PopUpDuration => popDuration;

        private void Start()
        {
            CanvasGroup.alpha = 0;
        }

        public void Initialize(string element, int percents)
        {
            elementText.text = element;
            percentsText.text = percents + "%";
        }

        public void Show()
        {
            gameObject.SetActive(true);
            CanvasGroup.alpha = 0;
            var sequence = DOTween.Sequence();
            sequence.Append(CanvasGroup.DOFade(1f, popDuration));
            //sequence.Insert(0f,transform.DOPunchScale(Vector3.one, popDuration));
            sequence.Play();
        }

        public void Hide()
        {
            CanvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        
        
        
    }
}