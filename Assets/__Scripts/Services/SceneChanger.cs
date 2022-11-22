using System;
using System.Collections.Generic;
using RSG;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Scripts
{
    public class SceneChanger
    {
        [Inject] private SplashScreenService splashScreenService;
        [Inject] private LocationManager locationManager;
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _scenesInUse = new();
        public string ActiveScene => SceneManager.GetActiveScene().name;

        public IPromise LoadSceneAsync(string sceneName, bool showSplash = true, bool resolveAfterSplash = true,
            bool isAdditive = false, bool changeAfterBoot = false)
        {
            var promise = new Promise();
            splashScreenService.Show()
                .Then(() => PerformSceneLoading(sceneName))
                .Then(() => changeAfterBoot ? locationManager.SwitchLocation(showSplash: false).Catch(Debug.LogException) : Promise.Resolved())
                .Then(() =>
                {
                    if (showSplash && !resolveAfterSplash)
                        promise.ResolveIfPending();
                })
                .Then(splashScreenService.Hide)
                .Then(() =>
                {
                    if (showSplash && resolveAfterSplash)
                        promise.ResolveIfPending();
                })
                .Catch(Debug.LogException)
                .Finally(promise.ResolveIfPending);

            return promise;
        }

        public IPromise LoadAddressableSceneAsync(string sceneName, bool setAsActive = true)
        {
            return PerformAddressablesSceneLoading(sceneName, isAdditive: setAsActive);
        }

        private IPromise PerformSceneLoading(string sceneName, bool isAdditive = false, bool setAsActive = true)
        {
            var promise = new Promise();
            try
            {
                var handle = SceneManager.LoadSceneAsync(sceneName,
                    isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                handle.allowSceneActivation = setAsActive;
                handle.completed += _ => promise.ResolveIfPending();

                //sceneInstance.ActivateAsync();
            }
            catch (Exception e)
            {
                promise.Reject(e);
                Console.WriteLine(e);
                throw;
            }

            return promise;
        }

        private IPromise PerformAddressablesSceneLoading(string sceneName, bool setAsActive = true,
            bool isAdditive = true)
        {
            var promise = new Promise();
            try
            {
                var handle = Addressables.LoadSceneAsync(sceneName,
                    isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                handle.CompletedTypeless += _ => promise.ResolveIfPending();

                if (setAsActive)
                    handle.Completed += a => SceneManager.SetActiveScene(a.Result.Scene);
                if (!_scenesInUse.ContainsKey(sceneName))
                    _scenesInUse.Add(sceneName, handle);
                //sceneInstance.ActivateAsync();
            }
            catch (Exception e)
            {
                promise.Reject(e);
                Console.WriteLine(e);
                throw;
            }

            return promise;
        }

        public IPromise ReleaseScene(string id)
        {
            var promise = new Promise();
            if (_scenesInUse.ContainsKey(id))
            {
                try
                {
                    GC.Collect();
                    AsyncOperationHandle handle = Addressables.UnloadSceneAsync(_scenesInUse[id]);
                    Debug.Log($"Addressables release:\nScene: {id}\nScenes in use: {_scenesInUse.ToString()}");
                    handle.Completed += _ =>
                    {
                        promise.ResolveIfPending();
                        _scenesInUse.Remove(id);
                        Resources.UnloadUnusedAssets();
                    };
                }
                catch (Exception e)
                {
                    var h = SceneManager.UnloadSceneAsync(id);
                    Debug.Log($"Regular release:\nScene: {id}\nScenes in use: {_scenesInUse.ToString()}");
                    h.completed += _ =>
                    {
                        _scenesInUse.Remove(id);
                        promise.ResolveIfPending();
                        Resources.UnloadUnusedAssets();
                    };
                    Debug.LogWarning(e);
                    promise.ResolveIfPending();
                   // throw;
                }
            }
            else
            {
                promise.Resolve();
            }

            return promise;
        }
    }
}