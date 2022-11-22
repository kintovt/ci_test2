using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RSG;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts
{
    public class UserProgressHolder : IInitializable, ITickable
    {
        [NonSerialized] private IModelLoader _loader;
        [NonSerialized] private IModelSaver _saver;

        [NonSerialized] private BoolReactiveProperty _initialized = new(false);
        public IReadOnlyReactiveProperty<bool> Initialized => _initialized;

        private UserProgress progress;
        private const string USER_PROGRESS_PATH = "UserProgress";

        private IPromiseTimer promiseTimer;

        public UserProgress Progress => progress;

        [Inject]
        public UserProgressHolder(IModelLoader loader, IModelSaver saver)
        {
            _loader = loader;
            _saver = saver;
        }

        public async void Initialize()
        {
            progress = await GetProgress();
            _initialized.Value = true;
            progress.saveRequest.AddListener(SaveState);
            promiseTimer = new PromiseTimer();
        }

        private async Task<UserProgress> GetProgress()
        {
            progress ??= await LoadState();
            progress ??= new UserProgress(new List<string>(), 100, null);
            return progress;
        }

        private async Task<UserProgress> LoadState()
        {
            var model = await _loader.Load<UserProgress>(USER_PROGRESS_PATH);
            return model;
        }

        public void SaveState()
        {
            _saver.Save(progress, USER_PROGRESS_PATH);
        }

        public void Tick()
        {
            promiseTimer?.Update(Time.deltaTime);
        }
    }
}