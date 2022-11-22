using System;
using System.Collections;
using RSG;
using UnityEngine;

namespace _Scripts
{
    public interface ITimeService
    {
        Coroutine StartCoroutine(IEnumerator coroutineBody);
        void StopCoroutine(Coroutine coroutine);
        Promise WaitForTime(float timeToWait);
        IPromise WaitForFrames(int framesToWait = 1);
        IPromise DoEveryFrameDuring(Action<float> toDo, float time);
        Promise DoEverySecondDuring(Action<float> toDo, float time);
    }
}