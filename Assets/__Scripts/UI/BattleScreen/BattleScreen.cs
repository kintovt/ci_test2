using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Zenject;

namespace _Scripts.UI
{
    public class BattleScreen : MonoBehaviour
    {
        [Inject] private AvatarSpawner avatarSpawner;

        public BattleCard[] battleCards;
        public ViewController viewController;
        public GameObject background;
        public GameObject tutorialScreen;
        public BattleCardsAnimator cardsAnimator;
        public TMP_Text resultText;
        public DanceEvents danceEvents;
        public int score = 0;
        void Awake()
        {
            danceEvents = viewController.clipSwitcher.gameObject.GetComponent<DanceEvents>();
        }

        public void StartBattle()
        {
            background.SetActive(true);
            tutorialScreen.SetActive(true);
        }

        public void HidePreview()
        {
            avatarSpawner.Animator.speed = 1f;
            cardsAnimator.gameObject.SetActive(true);
            tutorialScreen.SetActive(false);
            viewController.music.StartRandomBattleClip(1f);
        }
    }
}