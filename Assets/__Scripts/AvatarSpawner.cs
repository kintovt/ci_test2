using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using ReadyPlayerMe;
using RSG;
using Sirenix.OdinInspector;
using SRF;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class AvatarSpawner : MonoBehaviour
{
    //[Inject] private CameraManager cameraManager;

    [Inject] private UserProgressHolder progressHolder;
    [Inject] private SplashScreenService splashScreenService;

    [FoldoutGroup("Defaults"), SerializeField] private GameObject defaultBackGround;
    [FoldoutGroup("Defaults"), SerializeField] private GameObject defaultAvatar;
    [FoldoutGroup("Defaults"), SerializeField] private Vector3 defaultAvatarPosition = new Vector3(0f, 0f, 0.13f);
    [FoldoutGroup("Defaults"), SerializeField] private RuntimeAnimatorController defaultController;

    [FoldoutGroup("Configurations"), SerializeField] private string transformToTrackName = "Hips";

    public AvatarLights lights;
    
    private GameObject _currentAvatar;
    private List<GameObject> _loadedAvatars = new();

    public Transform Avatar => (_currentAvatar ??= defaultAvatar).transform;

    public Animator Animator => Avatar.GetComponent<Animator>() ?? Avatar.gameObject.AddComponent<Animator>();
    public QTEAvatarController QTEAvatarController => GetComponentInChildren<QTEAvatarController>();

    public UnityEvent<FailureEventArgs> OnFailed { get; } = new();
    public UnityEvent<ProgressChangeEventArgs> OnProgressChanged { get; } = new();
    public UnityEvent<CompletionEventArgs> OnCompleted { get; } = new();

    private static AvatarSpawner Instance;


    private void Awake()
    {
        Debug.Log("AvatarSpawner.Awake()");
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        _currentAvatar ??= defaultAvatar;
        Animator.runtimeAnimatorController = defaultController;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public IPromise LoadAvatar(string url)
    {
        Debug.Log("AvatarSpawner.LoadAvatar()");
        var promise = new Promise();
        var avatarLoader = new AvatarLoader();
        avatarLoader.OnProgressChanged += UpdateProgress;
        avatarLoader.OnFailed += Failed;
        avatarLoader.OnCompleted += OnAvatarLoaded;
        avatarLoader.OnCompleted += (_,_) => promise.ResolveIfPending();
        avatarLoader.LoadAvatar(url);
        PlayerPrefs.SetString("AvatarUrl", url);
        return promise;
    }

    private void Failed(object sender, FailureEventArgs e)
    {
        Debug.Log("AvatarSpawner.Failed()");
        OnFailed?.Invoke(e);
    }

    private void UpdateProgress(object sender, ProgressChangeEventArgs e)
    {
        //Debug.Log($"AvatarSpawner.UpdateProgress({e.Progress})");
        splashScreenService.GetProgressBar().value = e.Progress;
        OnProgressChanged?.Invoke(e);
    }

    private void OnAvatarLoaded(object sender, CompletionEventArgs args)
    {
        Debug.Log("AvatarSpawner.OnAvatarLoaded()");
        OnCompleted?.Invoke(args);
        SetupAvatar(args.Avatar);
    }

    private void SetupAvatar(GameObject avatar)
    {
        Debug.Log($"AvatarSpawner.SetupAvatar({avatar.name})");
        if (_currentAvatar)
            _currentAvatar.SetActive(false);
        _currentAvatar = avatar;
        _currentAvatar.AddComponent<EyeAnimationHandler>();
        _currentAvatar.SetLayerRecursive(LayerMask.NameToLayer("Avatar"));
        Avatar.SetParent(transform);
        ToStartPoint();
        var lookTarget = _currentAvatar.transform.FirstChildOrDefault(t => t.name == transformToTrackName);
        var cameraManager = GetComponent<CameraManager>();
        cameraManager.SetResultsPoint(lookTarget);
        cameraManager.SetPreviewCamera(lookTarget);
        cameraManager.LookAt(Avatar);
        //cameraManager.SetUpAvatarImage(_currentAvatar, defaultController);
        Animator.runtimeAnimatorController = defaultController;
    }

    public void ToStartPoint()
    {
        Debug.Log("AvatarSpawner.ToStartPoint()");
        Avatar.DOLocalMove(defaultAvatarPosition, 2f);
        Avatar.DOLocalRotate(Vector3.zero, 2f);
    }
    
    
}