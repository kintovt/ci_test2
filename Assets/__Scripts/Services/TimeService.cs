using System;
using System.Collections;
using _Scripts;
using RSG;
using UnityEngine;

namespace Infrastructure.Services.CoroutineService
{
    public class TimeService : MonoBehaviour, ITimeService
    {

        public static TimeService Instance;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public Promise WaitForTime(float timeToWait)
        {
            var result = new Promise();
            StartCoroutine(WaitForTimeCoroutine(result, timeToWait));
            return result;
        }

        public IPromise WaitForFrames(int framesToWait = 1)
        {
            var result = new Promise();
            StartCoroutine(WaitForFramesCoroutine(result, framesToWait));
            return result;
        }

        public IPromise DoEveryFrameDuring(Action<float> toDo, float time)
        {
            var result = new Promise();
            StartCoroutine(DoEveryFrameCoroutine(result, time, toDo, Time.time));
            return result;
        }
        

        public Promise DoEverySecondDuring(Action<float> toDo, float time)
        {
            var result = new Promise();
            var coroutine = StartCoroutine(DoEverySecondCoroutine(result, time, toDo, Time.time));
            result.Then(onResolved: () =>
            {
                if(coroutine != null) StopCoroutine(coroutine);
            });
            return result;
        }

        private IEnumerator WaitForTimeCoroutine(Promise completionPromise, float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            if (completionPromise.IsPending())
                completionPromise.Resolve();
        }

        private IEnumerator WaitForFramesCoroutine(Promise completionPromise, int framesToWait)
        {
            for (int i = 0; i < framesToWait; i++)
            {
                yield return null;
            }

            completionPromise.Resolve();
        }
        private IEnumerator DoEverySecondCoroutine(Promise completionPromise, float time, Action<float> toDo,
            float initialTime)
        {
            var progress = 0f;

            while (progress < 1f && completionPromise.CurState == PromiseState.Pending)
            {
                var timeGone = Time.time - initialTime;
                progress = Mathf.Min(timeGone/ time, 1f);
                toDo?.Invoke(time - timeGone);
                yield return new WaitForSeconds(1f);
            }
            completionPromise.Resolve();
        }



        private IEnumerator DoEveryFrameCoroutine(Promise completionPromise, float time, Action<float> toDo,
            float initialTime)
        {
            var progress = 0f;

            while (progress < 1f)
            {
                progress = Mathf.Min((Time.time - initialTime) / time, 1f);
                toDo?.Invoke(progress);
                yield return null;
            }

            completionPromise.Resolve();
        }
    }
}