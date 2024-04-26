using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System.Linq;
using TMPro;

public class SimpleAudio : MonoBehaviour
{
    [SerializeField, Tooltip("The audio source that should replay the captured audio.")]
    private AudioSource _playbackAudioSource = null;

    private GameObject systemMsgObj;
    private TextMeshProUGUI systemMsg;

    //Support Max 5mins of recording
    private const int AUDIO_CLIP_LENGTH_SECONDS = 300;

    private MLAudioInput.BufferClip mlAudioBufferClip;

    
    void Start()
    {
        systemMsgObj = GameObject.Find("System Message");
        systemMsg = GameObject.Find("System Message Text").GetComponent<TextMeshProUGUI>();

        OnEnable();
    }

    void OnEnable()
    {
        if (_playbackAudioSource == null)
        {
            Debug.LogError("PlaybackAudioSource is not set, adding component to " + gameObject.name);
            _playbackAudioSource = gameObject.AddComponent<AudioSource>();
            // _playbackAudioSource.pitch = _pitch;
            _playbackAudioSource.clip = null;
            _playbackAudioSource.loop = false;
        }
    }

    void OnDisable()
    {
        StopCapture();
        
    }

    public void StartMicrophone()
    {
        _playbackAudioSource.Stop();
        var captureType = MLAudioInput.MicCaptureType.VoiceCapture;
        var sampleRate = MLAudioInput.GetSampleRate(captureType);
        mlAudioBufferClip = new MLAudioInput.BufferClip(captureType, AUDIO_CLIP_LENGTH_SECONDS, sampleRate);
        
        // mlAudioBufferClip.OnReceivedSamples += DetectAudio;
    }

    public void StopCapture()
    {
        mlAudioBufferClip.Dispose();
        //Make AudioBufferClip playable
        _playbackAudioSource.clip = mlAudioBufferClip.FlushToClip();
        
        _playbackAudioSource.time = 0;
        // _playbackAudioSource.Play();

        mlAudioBufferClip = null;

        // Stop audio playback source and reset settings.
        // _playbackAudioSource.Stop();
        // _playbackAudioSource.time = 0;
        // _playbackAudioSource.pitch = 1;
        // _playbackAudioSource.loop = false;
        // _playbackAudioSource.clip = null;
    }
    
    private void DetectAudio(float[] samples)
    {
        
    }


    // private void OnPermissionGranted(string permission)
    // {
    //     StartMicrophone();
    //     Debug.Log($"Succeeded in requesting {permission}.");
    // }
}