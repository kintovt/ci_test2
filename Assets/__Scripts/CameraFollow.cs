using System;
using UnityEngine;

namespace _Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform follow;
        [SerializeField, Range(0, 20)] private float lerp = 0.8f;
        [SerializeField] private Vector3 initialPoint;
        [SerializeField] private bool x;
        private bool y;
        private bool z;

        private Vector3 newPos;


        public void SetActive(bool active) => enabled = active;
        public void Follow(Transform target)
        {
            follow = target;
        }

        public void ToStartPoint() => transform.position = initialPoint;
        private void Awake()
        {
            initialPoint = transform.position;
            newPos = initialPoint;
        }


        private void LateUpdate()
        {
            if (!follow) return;
            var point = new Vector3(follow.position.x, transform.position.y,
                transform.position.z);
            newPos = Vector3.Lerp(transform.position, point, lerp);
            transform.position = newPos;
        }
    }
}