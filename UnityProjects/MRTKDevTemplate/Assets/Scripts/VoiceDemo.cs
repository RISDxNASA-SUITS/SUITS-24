using UnityEngine;
using UnityEngine.XR.MagicLeap;
public class AudioEventExample : MonoBehaviour
{
    [SerializeField, Tooltip("The audio source that should replay the captured audio.")]
    private AudioSource _playbackAudioSource = null;

    private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();
    private MLAudioInput.StreamingClip mlAudioStreamingClip;
    private MLAudioInput.BufferClip mlAudioBufferClip;

    void Awake()
    {
        if (_playbackAudioSource == null)
        {
            Debug.LogError("PlaybackAudioSource is not set, adding component to " + gameObject.name);
            _playbackAudioSource = gameObject.AddComponent<AudioSource>();
        }
        permissionCallbacks.OnPermissionGranted += OnPermissionGranted;
        MLPermissions.RequestPermission(MLPermission.RecordAudio, permissionCallbacks);
    }

    void OnDestroy()
    {
        StopCapture();
        permissionCallbacks.OnPermissionGranted -= OnPermissionGranted;
    }

    private void StartMicrophone()
    {
        _playbackAudioSource.Stop();
        var captureType = MLAudioInput.MicCaptureType.VoiceCapture;
        mlAudioStreamingClip = new MLAudioInput.StreamingClip(MLAudioInput.MicCaptureType.VoiceCapture, 3, MLAudioInput.GetSampleRate(captureType));

        _playbackAudioSource.pitch = 1;
        _playbackAudioSource.clip = mlAudioStreamingClip.UnityAudioClip;
        _playbackAudioSource.loop = true;
        _playbackAudioSource.Play();
    }

    private void StopCapture()
    {
        mlAudioStreamingClip?.Dispose();
        mlAudioStreamingClip = null;
        mlAudioBufferClip?.Dispose();
        mlAudioBufferClip = null;
        // Stop audio playback source and reset settings.
        _playbackAudioSource.Stop();
        _playbackAudioSource.time = 0;
        _playbackAudioSource.pitch = 1;
        _playbackAudioSource.loop = false;
        _playbackAudioSource.clip = null;
    }

    private void OnPermissionGranted(string permission)
    {
        StartMicrophone();
        Debug.Log($"Succeeded in requesting {permission}.");
    }
}
