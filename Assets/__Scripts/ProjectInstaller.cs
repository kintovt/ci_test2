using System;
using Infrastructure.Services.CoroutineService;
using UnityEngine;
using Zenject;

namespace _Scripts
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private AvatarSpawner avatarRoot;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private LocationManager locationManager;
        [SerializeField] private SplashScreenService splashScreenService;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<FlowManager>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AddressablesManager>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SoundManager>().FromInstance(soundManager).AsSingle().NonLazy();
            Container.Bind<SplashScreenService>().FromInstance(splashScreenService).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UserProgressHolder>().FromNew().AsSingle().NonLazy();

            Container.Bind<LocationManager>().FromInstance(locationManager).AsSingle().NonLazy();
            Container.Bind<AvatarSpawner>().FromInstance(avatarRoot).AsSingle().NonLazy();
            Container.Bind<CameraManager>().FromInstance(cameraManager).AsSingle().NonLazy();
            Container.Bind<ITimeService>().To<TimeService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<IModelLoader>().To<ModelLoader>().FromNew().AsSingle().NonLazy();
            Container.Bind<IModelSaver>().To<ModelSaver>().FromNew().AsSingle().NonLazy();
            Container.Bind<SceneChanger>().FromNew().AsSingle().NonLazy();
            InstallVideoRecorder();
            // TODO: AudioSettings.Mobile.muteState
        }

        private void InstallVideoRecorder()
        {
            Container.Bind<IVideoRecorder>()
                .To<NatCorderController>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
#if UNITY_IOS || UNITY_EDITOR
            //Container.Bind<IVideoRecorder>().To<IOSVideoRecorder>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
#elif UNITY_ANDROID
            //throw new NullReferenceException("No video recorder for Android");
#endif
        }
    }
}