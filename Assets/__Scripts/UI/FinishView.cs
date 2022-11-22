using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace _Scripts
{
    public class FinishView : MonoBehaviour
    {
        [SerializeField] private Transform starImage;
        [SerializeField] private GameObject elipses;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private List<DanceElementsResultView> resultViews;
        [SerializeField] private GameObject elementsGroup;
        
        [SerializeField] private Button closeButton;
        [SerializeField] private Button shareButton;
        [SerializeField] private GameObject loadingImage;

        [SerializeField] private DanceSceneContext context;


        public UnityEvent close { get; } = new();
        
        public UnityEvent finishRecording { get; } = new();
        public UnityEvent share { get; } = new();
        private void OnEnable()
        {
            closeButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => context.IncrementDanceNumber());
            closeButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => Close());
            shareButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OnShare());

        }

        [Button]
        public void Open()
        {
            finishRecording?.Invoke();
            elementsGroup.SetActive(true);
            elementsGroup.GetComponent<CanvasGroup>().alpha = 0;
            starImage.localScale = Vector3.one;
            elipses.transform.localScale = Vector3.zero;
            elipses.SetActive(true);
            starImage.GetComponent<Image>().DOFade(1f, 0.1f);
            starImage.gameObject.SetActive(true);
            ShowPoints();
            var sequence = DOTween.Sequence();
            sequence.Append(starImage.GetComponent<Image>().DOFade(1f, 0.1f)).SetEase(Ease.OutBounce);
            sequence.AppendCallback(() => finishRecording?.Invoke());
            sequence.Insert(0f,starImage.DOScale(Vector3.one * 10f, 0.5f)).SetEase(Ease.OutBounce);
            sequence.Append(pointsText.transform.DOPunchScale(Vector3.one, 0.4f));
            sequence.Insert(0.7f, elipses.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutBounce));
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => ShowElements(onComplete: ShowButtons));
            sequence.Play();
        }

        public void Close()
        {  
            loadingImage.SetActive(false);
            pointsText.gameObject.SetActive(false);
            HideButtons(); 
            elipses.SetActive(false);
            elementsGroup.SetActive(false);
            close?.Invoke();
        }
        public void OnShare()
        {
            loadingImage.SetActive(true);
            share?.Invoke();
        }

        public void EnableShare()
        {
            shareButton.gameObject.SetActive(true);
            loadingImage.SetActive(false);
        }
        public void DisableShare()
        {
            shareButton.gameObject.SetActive(false);
        }
        

        private void ShowElements(Action onComplete = null)
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                elementsGroup.SetActive(true);
                elementsGroup.GetComponent<CanvasGroup>().alpha = 1f;
                elementsGroup.GetComponent<Animation>().Play();
            });
            foreach (var view in resultViews)
            {
                sequence.AppendCallback(view.Show);
                //sequence.AppendInterval(view.PopUpDuration * 3/4);
            }
            sequence.OnComplete(() => onComplete?.Invoke());
            sequence.Play();
        }

        private void ShowButtons()
        {
            closeButton.gameObject.SetActive(true);
            var sequence = DOTween.Sequence();
            sequence.Append(closeButton.targetGraphic.DOFade(1f, 0.2f));
            shareButton.gameObject.SetActive(true);
            sequence.Append(shareButton.image.DOFade(1f, 0.2f));
            sequence.Play();
        }

        private void HideButtons()
        {
            resultViews.ForEach(v => v.Hide());
            closeButton.gameObject.SetActive(false);
            shareButton.gameObject.SetActive(false);
        }

        public void ShowPoints()
        {
            pointsText.gameObject.SetActive(true);
        }
    }
}