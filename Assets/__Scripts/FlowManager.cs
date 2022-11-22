using System.Linq;
using _Scripts;
using ModestTree;
using RSG;
using UniRx;
using UnityEngine;
using Zenject;

public class FlowManager : IInitializable
{
    [Inject] private SceneChanger sceneChanger;
    [Inject] private ITimeService timeService;
    [Inject] private SplashScreenService _splashScreenService;
    [Inject] private UserProgressHolder _progress;
    [Inject] private AvatarSpawner _avatarSpawner;
    [Inject] private LocationManager locationManager;


    public void Initialize()
    {
        Debug.Log("FlowManager.Initialize()");
        Shader.WarmupAllShaders();
        timeService.WaitForTime(0.5f)
            .Then(_splashScreenService.Show)
            .Then(() =>
            {
                _avatarSpawner.gameObject.SetActive(true);
                EntryPoint();
            })
            .Catch(Debug.LogException);
    }

    private void EntryPoint()
    {
        Debug.Log("FlowManager.EntryPoint()");
        if (!_progress.Initialized.Value)
        {
            Debug.Log("FlowManager.EntryPoint(): progress not initialized");
            _progress.Initialized.Where(ready => ready).Subscribe(_ => EntryPoint()).AddTo(_avatarSpawner);
            return;
        }

        Debug.Log($"FlowManager.EntryPoint(): active scene is {sceneChanger.ActiveScene}");
        if (sceneChanger.ActiveScene == "EntryPoint")
        {
            if (_progress.Progress.AvatarURLs == null || _progress.Progress.AvatarURLs.IsEmpty())
            {
                Debug.Log("FlowManager.EntryPoint(): no avatar urls found in progress, loading avatar creation scene");
                LoadAvatarCreation();
            }
            else
            {
                Debug.Log("FlowManager.EntryPoint(): avatar urls found in progress, loading last used avatar, then loading room");
                LoadAvatar().Then(LoadRoom).Catch(Debug.LogException);
                //displayButton.gameObject.SetActive(false);
            }
        }
        else
        {
            _avatarSpawner.LoadAvatar(_progress.Progress.LastSelected ?? UserDefaults.avatarURL1);
            _splashScreenService.Hide();
        }
    }

    public void LoadAvatarCreation()
    {
        Debug.Log("FlowManager.LoadAvatarCreation()");
        sceneChanger.LoadSceneAsync("RPMWebView", resolveAfterSplash: false).Then(OnRPMSceneLoaded).Catch(Debug.LogException);
    }

    private void OnRPMSceneLoaded()
    {
        Debug.Log("FlowManager.OnRPMSceneLoaded()");
        var rpm = Object.FindObjectOfType<RPMController>(true);
        rpm.AvatarAccomplished.AddListener(LoadRoom);
    }

    private IPromise LoadAvatar()
    {
        Debug.Log("FlowManager.LoadAvatar()");
        var promise = _avatarSpawner.LoadAvatar(_progress.Progress.LastSelected ?? _progress.Progress.AvatarURLs.LastOrDefault());
        return promise;
    }

    private void LoadRoom()
    {
        Debug.Log("FlowManager.LoadRoom()");
        sceneChanger.LoadSceneAsync("Rapty");
    }
}