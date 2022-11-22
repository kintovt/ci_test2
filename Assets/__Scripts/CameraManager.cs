using System;
using Cinemachine;
using ReadyPlayerMe;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Zenject;

namespace _Scripts
{
    public class CameraManager : MonoBehaviour
    {
        [Inject] private IVideoRecorder videoRecorder;
        [Header("Settings")]
        [SerializeField] private float defaultBlendTime = 1f;
        [SerializeField] private float danceBlendTime = 4f;

        [Header("Dependencies")]
        [SerializeField] private Camera locationCamera;
        [SerializeField] private Camera avatarCamera;
        [SerializeField] private Camera previewCamera;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private CinemachineVirtualCamera menuVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera danceVirtualCamera;
        [SerializeField] private CameraFollow menuCameraFollowPoint;
        [SerializeField] private CameraFollow danceCameraFollowPoint;
        [SerializeField] private CinemachineVirtualCamera resultsVirtualCamera;
        [SerializeField] private CinemachineBrain brain;

        public ReactiveProperty<CameraPose> cameraPose = new(CameraPose.Menu);

        public Camera UICamera => uiCamera;

        private CinemachineVirtualCamera currentVirtualCamera;

        public enum CameraPose
        {
            Menu,
            Dance,
            Results,
        }

        private void Awake()
        {
            currentVirtualCamera = menuVirtualCamera;
        }

        private void Start()
        {
            danceCameraFollowPoint.SetActive(false);
            RegisterCameraForRecording(locationCamera);
        }

        private void OnEnable()
        {
            cameraPose
                .TakeUntilDisable(this)
                .Subscribe(SwitchPose);
        }

        public void LookAt(Transform transformToLook)
        {
            menuVirtualCamera.Follow = transformToLook;
            danceVirtualCamera.Follow = transformToLook;
            //danceCameraFollowPoint.Follow(transformToLook);
            //danceCameraFollowPoint.Follow(transformToLook);
            //danceCameraFollowPoint.SetActive(true);
        }

        public void SetPreviewCamera(Transform transformToLook)
        {
            Debug.Log($"CameraManager.SetPreviewCamera({transformToLook.name})");
            previewCamera.GetComponent<CameraLookAt>().target = transformToLook;
        }

        public void SetResultsPoint(Transform transformToLookAt)
        {
            resultsVirtualCamera.LookAt = transformToLookAt;
        }
        
        [Button]
        public void SwitchPose(CameraPose pose)
        {
            currentVirtualCamera.gameObject.SetActive(false);
            currentVirtualCamera = pose switch
            {
                CameraPose.Menu => menuVirtualCamera,
                CameraPose.Dance => danceVirtualCamera,
                CameraPose.Results => resultsVirtualCamera,
                _ => throw new ArgumentOutOfRangeException(nameof(pose), pose, null)
            };
            brain.m_DefaultBlend.m_Time = pose is CameraPose.Dance ? danceBlendTime : defaultBlendTime;
            if (pose is not CameraPose.Dance)
            {
                danceCameraFollowPoint.ToStartPoint();
                danceCameraFollowPoint.SetActive(false);
            }
            currentVirtualCamera.gameObject.SetActive(true);
        }

        public void SetAvatarCamera(bool active)
        {
            if (!active)
            {
                locationCamera.cullingMask |= 1 << LayerMask.NameToLayer("Avatar");
                //avatarCamera.enabled = false;
            }
            else
            {
                //avatarCamera.enabled = true;
                locationCamera.cullingMask &= ~ (1 << LayerMask.NameToLayer("Avatar"));
            }
        }

        private void RegisterCameraForRecording(Camera c)
        {
            videoRecorder.CameraToRead = c;
        }
        
    }
}