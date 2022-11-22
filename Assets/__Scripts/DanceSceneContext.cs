using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts
{
    public class DanceSceneContext : MonoBehaviour
    {
        [Inject] private AvatarSpawner avatarSpawner;
        [Inject] private CameraManager cameraManager;
        [Inject] private LocationManager locationManager;

        [SerializeField] private DanceConfig danceConfig;
        [SerializeField] private DanceController danceController;
        [SerializeField] private Canvas cameraCanvas; // TODO: TO view controller

        private int _selectedDanceIndex = 0;
        private DanceData _selectedDance;


        private void Awake()
        {
            cameraCanvas.worldCamera = cameraManager.UICamera;
        }

        private void Start()
        {
            locationManager
                .changeLocation
                .AsObservable()
                .Subscribe(OnChangeLocation)
                .AddTo(this);
        }

        private void OnChangeLocation(string location)
        {
            var d=  danceConfig.Dances.FirstOrDefault(dance => dance.Location == location);
            if (d != null)
                _selectedDance = d;
            else
            {
                Debug.LogError($"No dance for location: {location}");
            }
        }

        public void StartDance()
        {
            danceController.Animator = avatarSpawner.Animator;
            danceController.StartDance(_selectedDance, _selectedDanceIndex)
                .Then(avatarSpawner.ToStartPoint)
                .Catch(Debug.LogException);
        }
        
        public void IncrementDanceNumber()
        {
            _selectedDanceIndex++;
            if (_selectedDanceIndex >= _selectedDance.AnimationTriggers.Length)
                _selectedDanceIndex = 0;
            _selectedDanceIndex = Mathf.Clamp(_selectedDanceIndex, 0, _selectedDance.AnimationTriggers.Length - 1);
        }

    }
}