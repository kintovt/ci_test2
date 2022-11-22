using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts
{
    public class DanceTipsController : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [SerializeField] private TMP_Text moveText;
        [SerializeField] private float moveTextStart;
        [SerializeField] private float moveTextIn;
        [SerializeField] private float moveTextOut;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float showInterval = 2f;
        [SerializeField] private AnimatedText rateText;
        [SerializeField] private SpriteRenderer bgRateAnimation;



        private void Awake()
        {
            moveText.rectTransform.anchoredPosition =
                new Vector2(moveTextStart, moveText.rectTransform.anchoredPosition.y);
            rateText.SetActive(false);
            bgRateAnimation.gameObject.SetActive(false);

        }

        [Button]
        public void ShowMove(string text)
        {
            moveText.text = text;
            moveText.rectTransform.anchoredPosition =
                new Vector2(moveTextStart, moveText.rectTransform.anchoredPosition.y);
            var sequence = DOTween.Sequence();
            //sequence.Append(moveText.rectTransform.DOAnchorPosX(moveTextIn, animationDuration));
            sequence.AppendInterval(showInterval);
            //sequence.Append(moveText.rectTransform.DOAnchorPosX(moveTextOut, animationDuration));
            sequence.InsertCallback(animationDuration + showInterval,
                () =>
                {
                    _soundManager.PlaySFX();
                    rateText.SetActive(true);
                });
            sequence.InsertCallback(animationDuration + showInterval + 0.2f,
                () =>
                {
                    bgRateAnimation.gameObject.SetActive(true);
                    bgRateAnimation.GetComponent<Animation>().Play();
                });
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() =>
            {
                bgRateAnimation.gameObject.SetActive(false);
                rateText.gameObject.SetActive(false);
            });

        }
    }
}