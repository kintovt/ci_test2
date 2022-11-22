using DG.Tweening;
using ReadyPlayerMe;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Scripts
{
    public class RPMController : MonoBehaviour
    {
        private GameObject avatar;

        [Inject] private UserProgressHolder progressHolder;
        [Inject] private AvatarSpawner avatarSpawner;
            
        [SerializeField] private WebView webView;
        [SerializeField] private GameObject loadingLabel;
        [SerializeField] private Image loadingLabelImage;
        [SerializeField] private TMP_Text loadingProgressText;
        //[SerializeField] private Button displayButton;
        [FormerlySerializedAs("closeButton")] [SerializeField] private Button finishButton;

        [SerializeField, Tooltip("Uncheck if you don't want to continue editing the previous avatar, and make a completely new one.")]
        private bool keepBrowserSessionAlive = true;

        private int loadAttempts = 0;
        
        public UnityEvent AvatarAccomplished { get; } =  new();

        private void Start()
        {
            Debug.Log("RPMController.Start()");

            //displayButton.onClick.AddListener(DisplayWebView);
            finishButton.onClick.AddListener(NotifyAvatarAccomplished);
            if (!Application.isEditor)
            {
                if (webView == null) webView = FindObjectOfType<WebView>();
                webView.KeepSessionAlive = keepBrowserSessionAlive;
                DisplayWebView();
            }
            else
            {
                OnAvatarCreated(UserDefaults.avatarURL1);
            }
        }

        // Display WebView or create it if not initialized yet 
        private void DisplayWebView()
        {
            Debug.Log("RPMController.DisplayWebView()");

            if (webView.Loaded)
            {
                webView.SetVisible(true);
            }
            else
            {
                webView.CreateWebView();
                webView.OnAvatarCreated += OnAvatarCreated;
            }

            finishButton.gameObject.SetActive(false);
            //displayButton.gameObject.SetActive(false);
        }

        private void HideWebView()
        {
            Debug.Log("RPMController.HideWebView()");

            webView.SetVisible(false);
            finishButton.gameObject.SetActive(false);
            //displayButton.gameObject.SetActive(true);
        }
        
        private void NotifyAvatarAccomplished()
        {
            Debug.Log("RPMController.NotifyAvatarAccomplished()");
            AvatarAccomplished?.Invoke();
        }

        // WebView callback for retrieving avatar url
        private void OnAvatarCreated(string url)
        {
            Debug.Log("RPMController.OnAvatarCreated()");
            if (avatar) Destroy(avatar);
            webView.SetVisible(false);

            loadingLabel.SetActive(true);
            //displayButton.gameObject.SetActive(false);
            finishButton.gameObject.SetActive(false);


            avatarSpawner.OnProgressChanged
                .AsObservable()
                .TakeUntil(avatarSpawner.OnFailed.AsObservable())
                .TakeWhile(a => a.Progress <= 0.99)
                .Subscribe(UpdateProgress)
                .AddTo(this);
            avatarSpawner.OnCompleted
                .AsObservable()
                .TakeUntil(avatarSpawner.OnFailed.AsObservable())
                .Take(1)
                .Subscribe(Completed)
                .AddTo(this);
            avatarSpawner.OnFailed
                .AsObservable()
                .TakeUntil(avatarSpawner.OnCompleted.AsObservable())
                .Take(1)
                .Subscribe(Failed)
                .AddTo(this);
            avatarSpawner.LoadAvatar(url);
            progressHolder.Progress.AddAvatar(url);
            loadAttempts++;
        }

        private void UpdateProgress(ProgressChangeEventArgs e)
        {
            //Debug.Log("RPMController.UpdateProgress()");
            if (loadingLabelImage != null)
            {
                loadingLabelImage.fillAmount = e.Progress;
            }
            if (loadingProgressText != null)
            {
                if (e.Progress > 0.5f && loadingProgressText.color == Color.white)
                    loadingProgressText.DOColor(Color.black, 0.3f);
                loadingProgressText.text = e.Progress.ToString("P0");
            }
        }

        private void Failed(FailureEventArgs args)
        {
            Debug.Log("RPMController.Failed()");
            Debug.LogError(args.ToString());
            Debug.LogError($"Trying to load avatar again. Attempt: {loadAttempts}");
            if (loadAttempts > 3)
                return;
            OnAvatarCreated(args.Url);
        }

        // AvatarLoader callback for retrieving loaded avatar game object
        private void Completed(CompletionEventArgs args)
        {
            Debug.Log("RPMController.Completed()");
            //loadingLabel.SetActive(false);
            Debug.Log("Avatar Imported");
            NotifyAvatarAccomplished();
            //avatarAccomplished?.Invoke(Avatar);
        }

        private void OnDestroy()
        {
            //displayButton.onClick.RemoveListener(DisplayWebView);
            finishButton.onClick.RemoveAllListeners();
            
            //if (avatar) Destroy(avatar);
        }
    }
}