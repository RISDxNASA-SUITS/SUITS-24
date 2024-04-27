using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System.Linq;
using TMPro;

public class SimpleAudio : MonoBehaviour
{
    [SerializeField, Tooltip("The audio source that should replay the captured audio.")]
    private AudioSource _playbackAudioSource = null;

    //Support Max 5mins of recording
    private const int AUDIO_CLIP_LENGTH_SECONDS = 300;

    private MLAudioInput.BufferClip mlAudioBufferClip;

    private GameObject recordingOptionButtons;
    private GameObject recordingSystemMsg;

    private GameObject recordingButton;
    private GameObject startRecording;
    private GameObject recording;

    private GameObject timeCounterObj;
    private TextMeshProUGUI timeCounter;

    private float currentTime = 0f;

    private bool isTimeRunning;


    void Start()
    {
        recordingOptionButtons = GameObject.Find("Voice Option Buttons");
        recordingSystemMsg = GameObject.Find("System Message Text");

        recordingButton = GameObject.Find("Recording Button");
        startRecording = GameObject.Find("Start Recording");
        recording = GameObject.Find("Recording");

        timeCounterObj = GameObject.Find("Counter");
        timeCounter = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();

        recordingOptionButtons.SetActive(false);
        recordingSystemMsg.SetActive(false);
        recording.SetActive(false);


        OnEnable();
    }

    void Update()
    {
        if (isTimeRunning)
        {
            updateTime();
        }

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

        startRecording.SetActive(false);
        recording.SetActive(true);

        isTimeRunning = true;

        // mlAudioBufferClip.OnReceivedSamples += DetectAudio;
    }

    public void StopCapture()
    {
        recordingButton.SetActive(false);
        timeCounterObj.SetActive(false);
        recordingOptionButtons.SetActive(true);
        recordingSystemMsg.SetActive(true);

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

    private void updateTime()
    {
        currentTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime * 100f) % 100f);

        timeCounter.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    // private void OnPermissionGranted(string permission)
    // {
    //     StartMicrophone();
    //     Debug.Log($"Succeeded in requesting {permission}.");
    // }
}