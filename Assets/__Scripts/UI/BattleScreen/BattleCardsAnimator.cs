using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace _Scripts.UI
{
    public class BattleCardsAnimator : MonoBehaviour
    {
        public BattleScreen battleScreen;
        public Animator animator;
        public float speedScale = 1.3f;
        public float winCardShiftDistance = 50f;
        private void OnEnable()
        {
            animator.speed = speedScale;
            animator.enabled = true;
            animator.ResetTrigger("Next");
        }

        public void ReadyForDance()
        {
            battleScreen.viewController.StartDance();
            battleScreen.background.SetActive(false);
            animator.speed = 1f;
        }

        public void HideCardProgress(int cardIndex)
        {
            battleScreen.battleCards[cardIndex].transform.DOScale(1f, 0.5f);
            battleScreen.battleCards[cardIndex].progress.gameObject.SetActive(false);
            battleScreen.battleCards[cardIndex].progress.value = 0;
            battleScreen.danceEvents.OnDanceUpdated.RemoveListener(battleScreen.battleCards[cardIndex].UpdateSliderProgress);
            if(Random.value >= 0.5)
            {
                battleScreen.score++;
            }
            else
            {
                battleScreen.battleCards[cardIndex].SetRed();
            }
            animator.SetTrigger("Next");
        }
        public void ShowCardProgress(int cardIndex)
        {
            battleScreen.battleCards[cardIndex].transform.DOScale(1.2f, 0.5f);
            battleScreen.battleCards[cardIndex].progress.gameObject.SetActive(true);
            battleScreen.danceEvents.OnDanceUpdated.AddListener(battleScreen.battleCards[cardIndex].UpdateSliderProgress);
        }

        public void FinishDance()
        {
            animator.enabled = false;
            foreach(BattleCard card in battleScreen.battleCards)
            {
                if(!card.isRed)
                {
                    RectTransform rt = card.GetComponent<RectTransform>();
                    float tan = Mathf.Tan(Mathf.Deg2Rad * Mathf.Abs(rt.rotation.z));
                    float y = winCardShiftDistance / Mathf.Sqrt(Mathf.Pow(tan, 2) + 1);
                    float x = tan * y;
                    rt.DOAnchorPos(new Vector2(rt.anchoredPosition.x + Mathf.Sign(rt.rotation.z)*x, rt.anchoredPosition.y + y), 0.5f);
                }
            }
        }
    }
}
