using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Transform target;
    public float distance = 2.5f;
    private void Update()
    {
        //transform.LookAt(_target, Vector3.up);
        if (target != null)
        {
            var position = target.position;
            position.z += distance;
            position.y = transform.position.y;
            transform.position = position;
        }
    }
}
