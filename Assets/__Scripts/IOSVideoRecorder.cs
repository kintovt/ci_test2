using System;
using __Scripts;
using UniRx;
using UnityEngine;
using VoxelBusters.ReplayKit;
using Debug = UnityEngine.Debug;

public class IOSVideoRecorder : MonoBehaviour, IVideoRecorder
{
    
    private BoolReactiveProperty _initialized = new(false);
    public IReadOnlyReactiveProperty<bool> Initialized => _initialized;
    public static bool Available => IsAvailable();
    
    public Camera CameraToRead { get; set; }

    private ReactiveProperty<RecordingState> _recordingState = new();
    public IReadOnlyReactiveProperty<RecordingState> RecordingState => _recordingState;
    
    private void Start()
    { 
        if (Available)
        {
            ReplayKitManager.DidInitialise += OnInitialized;
            ReplayKitManager.Initialise();
            ReplayKitManager.DidRecordingStateChange += RecordingStateChanged;
        }

    }

    public void RequestAccept()
    {
        ReplayKitManager.PrepareRecording();
    }

    private void OnInitialized(ReplayKitInitialisationState state, string message)
    {
        Debug.Log("Received Event Callback : DidInitialiseEvent [State:" + state + " " + "Error:" + message);
        switch (state)
        {
            case ReplayKitInitialisationState.Success:
                Debug.Log("ReplayKitManager.DidInitialiseEvent : Initialisation Success");
                ReplayKitManager.SetMicrophoneStatus(enable: false);
                _initialized.Value = true;
                break;

            case ReplayKitInitialisationState.Failed:
                Debug.Log("ReplayKitManager.DidInitialiseEvent : Initialisation Failed with message["+message+"]");
                break;
            default:
                Debug.Log("Unknown State");
                break;
        }
    }
    
    public void StartRecording()
    {
        if (!ReplayKitManager.IsRecording())
        {
            ReplayKitManager.StartRecording();
            Debug.Log("Recording started");
        }
        else
        {
            Debug.LogError("Already recording on attempt to start a new recording");
        }
    }

    public void StopRecording()
    {
        if (ReplayKitManager.IsRecording())
        {
            ReplayKitManager.StopRecording();
            Debug.Log("Recording stopped");

        }
        else
        {
            Debug.LogError("Recording haven't been started on attempt to stop");
        }
    }


    public bool ShareRecording()
    {
        if(ReplayKitManager.IsPreviewAvailable())
        {
            ReplayKitManager.SharePreview();
            return true;
        }
        else
        {
            ReplayKitManager.DidRecordingStateChange += RecordingStateChanged;
            Debug.LogError("Recorded file not yet available. Please wait for ReplayKitRecordingState.Available status");
            return false;
        }
    }
    
    private void RecordingStateChanged(ReplayKitRecordingState state, string message) => _recordingState.Value = state.ToPlatformIndependent();

    private static bool IsAvailable()
    {
        var isRecordingAPIAvailable  =  ReplayKitManager.IsRecordingAPIAvailable();
        var message  = isRecordingAPIAvailable ? "Replay Kit recording API is available!" : "Replay Kit recording API is not available.";
        Debug.Log(message);
        return isRecordingAPIAvailable;
    }

}

public static class Extensions
{
    public static RecordingState ToPlatformIndependent(this ReplayKitRecordingState state)
    {
        return state switch
        {
            ReplayKitRecordingState.Started => RecordingState.Started,
            ReplayKitRecordingState.Stopped => RecordingState.Stopped,
            ReplayKitRecordingState.Failed => RecordingState.Failed,
            ReplayKitRecordingState.Available => RecordingState.Available,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}

public interface IVideoRecorder
{
    public IReadOnlyReactiveProperty<bool> Initialized { get; }
    public IReadOnlyReactiveProperty<RecordingState> RecordingState { get; }
    public static bool Available { get; }
    public Camera CameraToRead { get; set; }
    public void StartRecording();
    public void StopRecording();
    public bool ShareRecording();
    public void RequestAccept();
}
