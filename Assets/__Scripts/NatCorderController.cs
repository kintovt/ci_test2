using __Scripts;
using _Scripts;
using NatML.Recorders;
using NatML.Recorders.Clocks;
using NatML.Recorders.Inputs;
using NatSuite.Sharing;
using UniRx;
using UnityEngine;
using Zenject;

public class NatCorderController : MonoBehaviour, IVideoRecorder
{
    [Inject] private SoundManager _soundManager;
    private MP4Recorder _recorder;
    private RealtimeClock _clock;
    private CameraInput _cameraInput;
    private AudioInput _audioInput;


    private BoolReactiveProperty _initialized = new(true);

    public IReadOnlyReactiveProperty<bool> Initialized => _initialized;

    private ReactiveProperty<RecordingState> _state = new();
    public IReadOnlyReactiveProperty<RecordingState> RecordingState => _state;

    private Camera cameraToRead;
    private string path;

    public Camera CameraToRead
    {
        get => cameraToRead;
        set => cameraToRead = value;
    }

    public void StartRecording()
    {
        Debug.Log("NatCorderController.StartRecording()");
        if (CameraToRead == null)
        {
            Debug.LogError("No camera to read from. Initialize recorder with camera first");
            return;
        }
        
        _recorder = new MP4Recorder(Screen.width, Screen.height, 30, AudioSettings.outputSampleRate,  2);
        _clock = new RealtimeClock();
        _cameraInput = new CameraInput(_recorder, _clock, CameraToRead);

        if (CameraToRead.TryGetComponent(out AudioListener listener))
        {
            Debug.Log($"recording with audio. Sample rate:\n{AudioSettings.outputSampleRate.ToString()}");
            //_audioInput = new AudioInput(_recorder, _clock, listener);
        }
        _audioInput = new AudioInput(_recorder, _clock, _soundManager.MusicSource, mute: true );

        _state.Value = __Scripts.RecordingState.Started;
    }

    public async void StopRecording()
    {
        Debug.Log("NatCorderController.StopRecording()");
        _cameraInput?.Dispose();
        _audioInput?.Dispose();
        _state.Value = __Scripts.RecordingState.Stopped;
        if (_recorder != null)
        {
            Debug.Log("NatCorderController.StopRecording() finish writing");
            path = await _recorder.FinishWriting();
        } else
        {
            Debug.Log("NatCorderController.StopRecording() no recorder found!");
        }
        _state.Value = __Scripts.RecordingState.Available;
    }

    public bool ShareRecording()
    {
        if (string.IsNullOrEmpty(path)) return false;
        var payload = new SharePayload();
        payload.AddMedia(path);
        _ = payload.Commit();
        return true;
    }

    public void RequestAccept()
    {
    }
}