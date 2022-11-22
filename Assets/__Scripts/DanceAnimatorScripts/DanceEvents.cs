using UnityEngine.Events;
using UnityEngine;

public class DanceEvents : MonoBehaviour
{
    public UnityEvent OnDanceFinished;
    public UnityEvent OnDanceStarted;
    public UnityEvent<float> OnDanceUpdated;
}
