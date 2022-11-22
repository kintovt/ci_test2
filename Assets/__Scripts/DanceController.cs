using System.Collections;
using Infrastructure.Services.CoroutineService;
using RSG;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace _Scripts
{
    public class DanceController : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private LocationManager locationManager;
        [Inject] private UserProgressHolder progressHolder;

        [SerializeField] private DanceTipsController tips;
        [SerializeField] private Animator animator;
        [SerializeField] private float totalDuration = 20f;


        public float TotalDuration => totalDuration;
        private Promise current;
        private Coroutine _consumeCoroutine;


        public Animator Animator
        {
            get => animator;
            set => animator = value;
        }

        public UnityEvent danceCompleted  = new();

        private static readonly int StartTrigger = Animator.StringToHash("start_resort_town");
        private static readonly int LoopDance = Animator.StringToHash("loop_dance");
        private DanceData _dance;
        private static readonly int LoopDanceTrigger = Animator.StringToHash("loop_dance_trigger");

        public IPromise StartDance(DanceData dance, int danceNumber = 0)
        {
            current = new Promise();
            _dance = dance;
            //animator.runtimeAnimatorController = config.AnimatorController;
            //var triggerName = $"{config.AnimationTrigger}";
            // if (danceNumber != 0)
            //     triggerName += "_" + danceNumber;
            //animator.SetTrigger(triggerName);
            animator.SetTrigger(dance.AnimationTriggers[Mathf.Clamp(0,danceNumber, dance.AnimationTriggers.Length-1)]);
            //animator.SetBool(LoopDance, true);
            
            
            // foreach (var move in config.Moves)
            // {
            //     SetupTip(move);
            // }

            //totalDuration = config.Duration;
            if (dance.Audio != null)
            {
                _soundManager.PlayAudio(dance.Audio, dance.TrackStart);
            }
            //_consumeCoroutine = StartCoroutine(ConsumeStaminaCoroutine(totalDuration));
            // current = TimeService.Instance.WaitForTime(totalDuration)
            //     .Then(() =>
            //     {
            //         _soundManager.PauseAudio();
            //         danceCompleted?.Invoke();
            //
            //     }).Catch(Debug.LogException);
            return current;
        }

        public void FinishDance()
        {
            _soundManager.PauseAudio();
            danceCompleted?.Invoke();
            current?.ResolveIfPending();
        }

        private IEnumerator ConsumeStaminaCoroutine(float duration)
        {
            var seconds = 0;
            int staminaConsumed = 0;
            while (seconds < duration - 1f)
            {
                staminaConsumed += Mathf.RoundToInt( 20f / (duration/2));
                progressHolder.Progress.ConsumeStamina(Mathf.RoundToInt(20f/(duration/2)));
                yield return new WaitForSeconds(2f);
                ++seconds;
            }
            if (staminaConsumed < 20)
            {
                progressHolder.Progress.ConsumeStamina(20 - staminaConsumed);
            }
        }

        private IPromise SetupTip(Move move)
        {
            return TimeService.Instance.WaitForTime(move.timeStamp)
                .Then(() =>
                {
                    tips.ShowMove(move.element);
                })
                .Catch(Debug.LogException);
        }
    }
}