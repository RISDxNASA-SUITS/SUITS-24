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

    [SerializeField]
    private Sprite _startSprite = null;
    [SerializeField]
    private Sprite _stopSprite = null;

    //Support Max 5mins of recording
    private const int AUDIO_CLIP_LENGTH_SECONDS = 300;

    private MLAudioInput.BufferClip mlAudioBufferClip;

    private GameObject recordingOptionButtons;
    private GameObject recordingSystemMsg;

    private GameObject recordingButton;
    private Image recordingImage;

    private GameObject timeCounterObj;
    private TextMeshProUGUI timeCounter;

    private float currentTime = 0f;

    private IEnumerator timeUpdateIEnum;


    void Start()
    {
        recordingOptionButtons = GameObject.Find("Voice Option Buttons");
        recordingSystemMsg = GameObject.Find("System Message Text");

        recordingButton = GameObject.Find("Recording Button");
        recordingImage = recordingButton.transform.Find(
            "Frontplate/AnimatedContent/Icon/UIButtonSpriteIcon").GetComponent<Image>();

        // recordingImage = GameObject.Find("Recording Button Image").GetComponent<Image>();
        recordingImage.sprite = _startSprite;

        timeCounterObj = GameObject.Find("Counter");
        timeCounter = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();

        recordingOptionButtons.SetActive(false);
        recordingSystemMsg.SetActive(false);

        OnEnable();
    }

    private IEnumerator updateTime()
    {
        while (true) 
        {
            currentTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            int milliseconds = Mathf.FloorToInt((currentTime * 100f) % 100f);

            timeCounter.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

            yield return new WaitForSeconds(10 * 0.001f); // 10ms
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

        if (timeUpdateIEnum != null)
        {
            StopCoroutine(timeUpdateIEnum);
        }
    }

    public void StartMicrophone()
    {
        _playbackAudioSource.Stop();
        var captureType = MLAudioInput.MicCaptureType.VoiceCapture;
        var sampleRate = MLAudioInput.GetSampleRate(captureType);
        mlAudioBufferClip = new MLAudioInput.BufferClip(captureType, AUDIO_CLIP_LENGTH_SECONDS, sampleRate);

        recordingImage.sprite = _stopSprite;
        // mlAudioBufferClip.OnReceivedSamples += DetectAudio;

        StartCoroutine((timeUpdateIEnum = updateTime()));
    }

    public void StopCapture()
    {
        uiRecordingToConfirmation();

        Debug.Log("Calling discardRecordingFlush");
        discardRecordingFlush();
        Debug.Log("Called discardRecordingFlush");

        currentTime = 0f;
        timeCounter.text = "00:00:00";

        //only for deploy testing
        // discardRecordingFlush();
        _playbackAudioSource.Play();

        // Stop audio playback source and reset settings.
        // _playbackAudioSource.Stop();
        // _playbackAudioSource.time = 0;
        // _playbackAudioSource.pitch = 1;
        // _playbackAudioSource.loop = false;
        // _playbackAudioSource.clip = null;
    }

    public void DiscardCallback()
    {
        // Throw away the buffer clip
        // discardRecordingNoFlush();
        
        // UI: Go back to first page
        uiConfirmationToRecording();
    }

    public void SaveCallback()
    {
        // discardRecordingFlush();

        //TODO: Post _playbackAudioSource.clip to server

        // UI: Go back to first page
        uiConfirmationToRecording();
    }

    private void uiRecordingToConfirmation()
    {
        recordingButton.SetActive(false);
        timeCounterObj.SetActive(false);

        recordingOptionButtons.SetActive(true);
        recordingSystemMsg.SetActive(true);
    }

    private void uiConfirmationToRecording()
    {
        recordingButton.SetActive(true);
        timeCounterObj.SetActive(true);

        recordingOptionButtons.SetActive(false);
        recordingSystemMsg.SetActive(false);
    }

    private void discardRecordingFlush()
    {
        _playbackAudioSource.clip = mlAudioBufferClip?.FlushToClip();
        discardRecordingNoFlush();
    }

    private void discardRecordingNoFlush()
    {
        _playbackAudioSource.time = 0;
        mlAudioBufferClip?.Dispose();
        mlAudioBufferClip = null;


        Debug.Log("Stopping coroutine...");
        StopCoroutine(timeUpdateIEnum);
        timeUpdateIEnum = null;
        Debug.Log("Stopped coroutine.");

        currentTime = 0f;
        timeCounter.text = "00:00:00";

        recordingImage.sprite = _startSprite;
    }

    private void DetectAudio(float[] samples)
    {

    }

    // private void updateTime()
    // {
    //     currentTime += Time.deltaTime;

    //     int minutes = Mathf.FloorToInt(currentTime / 60f);
    //     int seconds = Mathf.FloorToInt(currentTime % 60f);
    //     int milliseconds = Mathf.FloorToInt((currentTime * 100f) % 100f);

    //     timeCounter.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    // }

    // private void OnPermissionGranted(string permission)
    // {
    //     StartMicrophone();
    //     Debug.Log($"Succeeded in requesting {permission}.");
    // }
}