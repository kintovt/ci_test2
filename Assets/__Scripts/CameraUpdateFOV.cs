using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraUpdateFOV: MonoBehaviour
    {
        [SerializeField, Required] private Camera target;
        [SerializeField] private Camera _camera;

        private Camera Camera
        {
            get
            {
                _camera ??= GetComponent<Camera>();
                return _camera;
            }
        }
        
        private void LateUpdate()
        {
            Camera.fieldOfView = target.fieldOfView;
        }

        private void OnValidate()
        {
            LateUpdate();
        }
    }
}