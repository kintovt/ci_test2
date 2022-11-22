using System;
using System.Collections;
using System.Threading.Tasks;
using __Scripts;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.UI
{
    public class ViewController : MonoBehaviour
    {
        [Inject] private IVideoRecorder videoRecorder;
        [Inject] private CameraManager cameraManager;
        [Inject] private FlowManager flowManager;
        [Inject] private LocationManager locationManager;
        [Inject] private UserProgressHolder progressHolder;
        [Inject] private AvatarSpawner avatarSpawner;

        [SerializeField] private GameObject header;
        [SerializeField] private Button startBattleButton;
        [SerializeField] public BattleScreen battleScreen;
        [SerializeField] private GameObject pointsCounterBlock;
        //
        [SerializeField] private Button changeAvatarButton;
        [SerializeField] private Button selectCardsButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button finishButton;
        [SerializeField] private CanvasGroup bottomButtonsCG;
        [SerializeField] private StaminaBar staminaBar;
        [SerializeField] private CountDown countDown;

        [SerializeField] public CardsScreen cardsScreen;

        [SerializeField] private PointsMock score;
        [SerializeField] private UIMock timer;

        [Header("UI"), SerializeField]
        private GameObject goRaptyText;

        [SerializeField] private GameObject yellowSplash;
        [SerializeField] private GameObject locationSplashScreen;
        [SerializeField] private Slider locationSplashScreenProgressBar;
        [Header("Scripts"), SerializeField] private DanceSceneContext danceSceneContext;
        [SerializeField] public DanceController danceController;
        [SerializeField] private FinishView finishView;
        [SerializeField] private AnimatorClipSwitcher _clipSwitcher => avatarSpawner.GetComponent<AnimatorClipSwitcher>();
        [SerializeField] private AnimatorSpeedController _speedController => avatarSpawner.GetComponent<AnimatorSpeedController>();
        [HideInInspector] public AnimatorClipSwitcher clipSwitcher { get => _clipSwitcher; }
        [SerializeField] private AnimationClip _idleAnimation;
        public MusicController music; 

        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Dancing = Animator.StringToHash("Dancing");
        private static readonly int WavingIdle = Animator.StringToHash("waving");
        private static readonly int Angry = Animator.StringToHash("angry");

        private IEnumerator Start()
        {
            cardsScreen.InitializeMotionCardsData();
            Debug.Log("ViewController.Start()");
            clipSwitcher.GetAnimatorOverrideController();
            OpenMenu();
            music.StartMainMenuClip(2f);
            yield return new WaitForSeconds(2f);
            avatarSpawner.Animator.SetBool(Idle, true);
            SetAvatarRotation();
            _clipSwitcher.viewController = this;
            
        }

        private void OnEnable()
        {
            Debug.Log("ViewController.OnEnable()");
            startBattleButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OnStartBattleButton());
            avatarSpawner.OnProgressChanged.AsObservable().TakeUntil(avatarSpawner.OnFailed.AsObservable()).TakeWhile(a => a.Progress <= 0.99).Subscribe(a => UpdateProgress(a));
            //

            staminaBar.SetActive(false);
            finishButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => danceController.FinishDance()) ;
            //staminaBar.UpdateValue(progressHolder.Progress.Stamina);
            //acceptButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OpenCardsScreen());
            startButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => StartDance());
            changeAvatarButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => ChangeAvatar());
            selectCardsButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(_ => OpenCardsScreen());
            danceController.danceCompleted.AsObservable().TakeUntilDisable(this).Subscribe(_ => OnDanceCompleted());
            finishView.close.AsObservable().TakeUntilDisable(this).Subscribe(_ =>
            {
                music.StartMainMenuClip(2f);
                OpenMenu();
                SwitchLocation(showSplash: true, isDefaultLocation: true);
                avatarSpawner.Animator.SetBool("Idle", true);
                battleScreen.resultText.gameObject.SetActive(false);
                battleScreen.cardsAnimator.gameObject.SetActive(false);
                foreach (BattleCard card in battleScreen.battleCards)
                    card.SetBlack();
                battleScreen.score = 0;
            });
            finishView.close.AsObservable().TakeUntilDisable(this).Subscribe(_ =>
            {
                avatarSpawner.Animator.SetBool(Idle, true);
            });
            finishView.finishRecording.AsObservable().TakeUntilDisable(this).Subscribe(_ => StopRecording());
            finishView.share.AsObservable().TakeUntilDisable(this).Subscribe(_ => Share());
            //progressHolder.Progress.staminaLevelChanged.AsObservable().TakeUntilDisable(this).Subscribe(staminaBar.UpdateValue).AddTo(this);

            cardsScreen.onPreviewDisplayed.AsObservable().TakeUntilDisable(this).Subscribe(data => PlayMotionClip(data));
            cardsScreen.onCardsSelected.AsObservable().TakeUntilDisable(this).Subscribe(_ => OnCardsScreenClosed());

        }

        private void SwitchLocation(bool showSplash = true, bool isDefaultLocation = false)
        {
            Debug.Log("ViewController.SwitchLocation()");
            cameraManager.SetAvatarCamera(true);
            bottomButtonsCG.DOFade(0f, 0.5f);
            bottomButtonsCG.blocksRaycasts = false;
            bottomButtonsCG.interactable = false;

            locationSplashScreen.SetActive(true);
            locationSplashScreenProgressBar.value = 0;
            var tween = DOTween.To(() => locationSplashScreenProgressBar.value, x => locationSplashScreenProgressBar.value = x, 1f, UnityEngine.Random.Range(1.4f, 2.7f));
            locationManager.SwitchLocation(showSplash : true, isDefaultLocation : isDefaultLocation)
                .Then(() => 
                { 
                    cameraManager.SetAvatarCamera(false); 
                    tween.Kill(); locationSplashScreenProgressBar.value = 1;
                    avatarSpawner.Avatar.position = Vector3.zero;
                    if (!isDefaultLocation) StartBattle(); 
                })
                .Catch(Debug.LogException)
                .Finally(() =>
                {
                    locationSplashScreen.SetActive(false);
                    bottomButtonsCG.DOFade(1f, 0.5f);
                    bottomButtonsCG.interactable = true;
                    bottomButtonsCG.blocksRaycasts = true;
                });
        }

        private void ChangeAvatar()
        {
            Debug.Log("ViewController.ChangeAvatar()");
            flowManager.LoadAvatarCreation();
        }

        private void StopRecording()
        {
            Debug.Log("ViewController.StopRecording()");
            if (videoRecorder.RecordingState.Value == RecordingState.Started)
                videoRecorder.StopRecording();
        }

        private void Share()
        {
            Debug.Log("ViewController.Share()");
            finishView.DisableShare();

            if (videoRecorder.ShareRecording())
            {
                finishView.EnableShare();
            }
            else if (videoRecorder.RecordingState.Value is RecordingState.Failed)
            {
                finishView.EnableShare();
                finishView.DisableShare();
            }
            else
            {
                Debug.Log("Recording is not ready.\nWaiting for recording to prepare");
                videoRecorder.RecordingState
                    .TakeUntil(finishView.close.AsObservable())
                    .Where(state => state == RecordingState.Available)
                    .Take(1)
                    .Subscribe(_ => Share());
            }
        }

        private void OnDanceCompleted()
        {
            Debug.Log("ViewController.OnDanceCompleted()");
            score.StopIncrement();
            battleScreen.resultText.text = battleScreen.score >= 2 ? String.Format("You {0}\n{1} of 4", "win", battleScreen.score) : String.Format("You {0}\n{1} of 4", "lose", battleScreen.score);
            battleScreen.resultText.gameObject.SetActive(true);
            battleScreen.cardsAnimator.animator.Play("BattleCardsStraightToBow");
            battleScreen.score = 0;
            music.StopMusic(2f);
            //avatarSpawner.Animator.SetTrigger("IdleTriggerSmooth");
            OpenResults();
        }

        private void OpenResults()
        {
            Debug.Log("ViewController.OpenResults()");

            startBattleButton.gameObject.SetActive(false);
            pointsCounterBlock.gameObject.SetActive(false);

            finishButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            staminaBar.SetActive(false);
            selectCardsButton.gameObject.SetActive(false);
            changeAvatarButton.gameObject.SetActive(false);
            changeAvatarButton.transform.parent.gameObject.SetActive(false);
            score.SetActive(false);

            cameraManager.cameraPose.Value = CameraManager.CameraPose.Results;
            finishView.Open();
            cameraManager.SetAvatarCamera(true);
            //avatarSpawner.Animator.SetTrigger(Angry);
            avatarSpawner.Animator.SetBool("Idle", true);
            avatarSpawner.Animator.SetBool("Sequence", false);
            avatarSpawner.Animator.SetBool("Battle",false);
        }

        private void OpenMenu()
        {
            Debug.Log("ViewController.OpenMenu()");
            clipSwitcher.SetIdleClip(clipSwitcher.clips[0]);
            avatarSpawner.Animator.SetTrigger("IdleTrigger");
            startBattleButton.gameObject.SetActive(true);
            startBattleButton.interactable = cardsScreen.GetSelectedCards() != null;
            pointsCounterBlock.gameObject.SetActive(true);

            cameraManager.SetAvatarCamera(false);
            SetAvatarRotation();
            cameraManager.cameraPose.Value = CameraManager.CameraPose.Menu;
            header.SetActive(true);

            //acceptButton.gameObject.SetActive(true);
            //staminaBar.SetActive(true);
            selectCardsButton.gameObject.SetActive(true);
            changeAvatarButton.gameObject.SetActive(true);
            changeAvatarButton.transform.parent.gameObject.SetActive(true);

            finishButton.gameObject.SetActive(false);
            score.SetActive(true);
            score.StopIncrement();
            goRaptyText.SetActive(false);
            //yellowSplash.SetActive(false);
            avatarSpawner.lights.SetMenuLightsOn();
        }

        private async void OpenCardsScreen()
        {
            Debug.Log("ViewController.OpenCardsScreen()");

            startBattleButton.gameObject.SetActive(false);
            score.SetActive(false);
            pointsCounterBlock.gameObject.SetActive(false);

            SetAvatarRotation();
            cardsScreen.gameObject.SetActive(true);
            await Task.Delay(100);
        }

        private void PlayMotionClip(MotionCardData data)
        {
            Debug.Log("ViewController.PlayMotionClip()");
            SetAvatarRotation();
            avatarSpawner.Animator.SetBool("Idle", false);
            _clipSwitcher.SetDanceClip(data.clip);
            avatarSpawner.lights.SetCardLightsOn(data.rarity);
        }
        
        private void StartBattle()
        {
            Debug.Log("ViewController.StartBattle()");
            startBattleButton.gameObject.SetActive(false);
            pointsCounterBlock.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            header.SetActive(false);
            staminaBar.SetActive(false);
            selectCardsButton.gameObject.SetActive(false);
            changeAvatarButton.gameObject.SetActive(false);
            changeAvatarButton.transform.parent.gameObject.SetActive(false);

            avatarSpawner.Animator.SetBool("Idle", true);
            clipSwitcher.SetIdleClip(clipSwitcher.clips[1]);
            avatarSpawner.Animator.SetTrigger("IdleTrigger");
            videoRecorder.RequestAccept();

            cameraManager.cameraPose.Value = CameraManager.CameraPose.Dance;
            battleScreen.StartBattle();
            avatarSpawner.Animator.speed = 0;
        }


        public async void StartDance()
        {
            Debug.Log("ViewController.StartDance()");

          

            //videoRecorder.StartRecording();
            //await countDown.StartCountDown(3);
            cameraManager.SetAvatarCamera(true);
            goRaptyText.SetActive(true);
            goRaptyText.GetComponent<Graphic>().DOFade(1f, 0.1f);
            //yellowSplash.GetComponent<Graphic>().DOFade(1f, 0.1f);
            //yellowSplash.transform.DOScale(7f, 0.1f);
            //yellowSplash.SetActive(true);

            //PlayMotionClip(cardsScreen.GetSelectedCards()[0].clip);
            
            avatarSpawner.Animator.SetBool("Sequence", true);
            avatarSpawner.Animator.SetBool("Idle", false);
            
            
            _clipSwitcher.SetDanceAnimations(cardsScreen.GetSelectedCards());
            _clipSwitcher.StartPlaying();
            avatarSpawner.Animator.SetBool("Battle", true);
            avatarSpawner.Animator.SetTrigger("DanceTriggerSmooth");
            //avatarSpawner.QTEAvatarController.GetComponent<AnimatorClipSwitcher>().SetAnimationClip(cardsScreen.GetSelectedCards()[0].clip);

            await Task.Delay(1300);
            goRaptyText.GetComponent<Graphic>().DOFade(0f, 0.1f);
            //yellowSplash.GetComponent<Graphic>().DOFade(0f, 0.1f);
            //yellowSplash.transform.DOScale(1f, 0.1f);
            await Task.Delay(200);
            cameraManager.SetAvatarCamera(false);
            goRaptyText.SetActive(false);
            //yellowSplash.SetActive(false);
            //staminaBar.SetActive(true);
            //danceSceneContext.StartDance();
            score.SetActive(true);

            videoRecorder.StartRecording();
            // finishButton.gameObject.SetActive(true);
        }

        private void OnCardsScreenClosed()
        {
            Debug.Log("ViewController.OnCardsScreenClosed()");
            avatarSpawner.Animator.SetBool("Idle", true);
            //avatarSpawner.Animator.SetTrigger("IdleTrigger");
            _clipSwitcher.SetIdleClip(_idleAnimation);
            cardsScreen.gameObject.SetActive(false);
            for (int i = 0; i < cardsScreen._slotsBlock._slots.Length; i++)
                battleScreen.battleCards[i].FillCard(cardsScreen._slotsBlock._slots[i].data, cardsScreen._slotsBlock._slots[i]._preview.texture); 
            OpenMenu();
        }

        private void SetAvatarRotation()
        {
            avatarSpawner.Avatar.transform.localEulerAngles = new Vector3();
        }

        public void OnStartBattleButton()
        {
            if (cardsScreen.GetSelectedCards() == null)
            {
                OpenCardsScreen();
            } else
            {
                music.StopMusic(2f);
                SwitchLocation(showSplash: false, isDefaultLocation: false);
            }
        }

        private void UpdateProgress(ReadyPlayerMe.ProgressChangeEventArgs a)
        {
            avatarSpawner.GetComponent<LocationManager>().GetSplashScreen().GetProgressBar().value = a.Progress;
        }
    }
}