using System.Collections.Generic;
using System.Threading.Tasks;
using RSG;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Scripts
{
    public class LocationManager : MonoBehaviour
    {
        [Inject] private SceneChanger sceneChanger;
        
        [SerializeField] private GameObject defaultLocation;
        [SerializeField] private SplashScreenService avatarBackgroundSplash;
        [SerializeField] private List<string> locationScenes;
        [SerializeField] private GameObject defaultLights;

        private GameObject currentLocation;
        private string currentLocationScene;
        private int currentIndex = 0;

        public UnityEvent<string> changeLocation { get; } = new();

        public void Start()
        {
            Debug.Log("LocationManager.Start()");
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log("LocationManager.OnSceneUnloaded()");
            if (scene.name == "Rapty")
            {
                currentLocationScene = "";
            }
        }

        public void OnDestroy()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

        }

        public IPromise SwitchLocation(bool showSplash = true, bool isDefaultLocation = false)
        {
            Debug.Log("LocationManager.SwitchLocation()");
            var promise = new Promise();
            currentIndex++;
            var locationSceneName = isDefaultLocation ? "Rapty" : locationScenes[Random.Range(0, locationScenes.Count)];
            if (locationSceneName == currentLocationScene)
                return Promise.Resolved();
            if (!showSplash)
            {
                return SetUpLocation(locationSceneName, isDefaultLocation);
            }
            avatarBackgroundSplash.Show()
                .Then(() => SetUpLocation(locationSceneName, isDefaultLocation))
                .Then(avatarBackgroundSplash.Hide)
                .Then(promise.ResolveIfPending)
                .Catch(Debug.LogException)
                .Finally(() =>
                {
                    promise.ResolveIfPending();
                });
            return promise;
        }

        public IPromise SetUpLocation(string reference, bool isDefaultLocation = false)
        {
            Debug.Log("LocationManager.SetUpLocation()");
            if (NeedToRelease(currentLocationScene))
                sceneChanger.ReleaseScene(currentLocationScene);
            currentLocationScene = reference;
            if (isDefaultLocation)
            {
                defaultLocation.SetActive(true);
                defaultLights.SetActive(true);
                return Promise.Resolved();
            }
            else
            {
                defaultLocation.SetActive(false);
                defaultLights.SetActive(false);
                var promise = sceneChanger.LoadAddressableSceneAsync(reference);
                if (currentLocation)
                    currentLocation.SetActive(false);
                promise.Then(() => changeLocation?.Invoke(reference)).Catch(Debug.LogException);
                return promise;
            }
        }

        public async Task SetUpLocation(AssetReference reference)
        {
            GameObject current = currentLocation;
            currentLocation = await reference.InstantiateAsync(transform).Task;
            if (current)
                Destroy(current);
            currentLocation.SetActive(true);
            defaultLocation.SetActive(false);
        }

        public SplashScreenService GetSplashScreen()
        {
            return avatarBackgroundSplash;
        }

        private static bool NeedToRelease(string sceneName)
        {
            var need = false;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                {
                    need = true;
                }
            }

            return need;
        }
    }
}