using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System.Linq;
using TMPro;
using MixedReality.Toolkit.UX;

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

    [SerializeField]
    private GameObject recordingOptionButtons;
    [SerializeField]
    private GameObject recordingSystemMsg;

    [SerializeField]
    private GameObject recordingButton;
    [SerializeField]
    private PressableButton recordingPressableButton;
    [SerializeField]
    private Image recordingImage;

    [SerializeField]
    private GameObject timeCounterObj;
    [SerializeField]
    private TextMeshProUGUI timeCounter;

    private float currentTime = 0f;

    private IEnumerator timeUpdateIEnum;

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
            Debug.LogWarning("PlaybackAudioSource is not set, adding component to " + gameObject.name);
            _playbackAudioSource = gameObject.AddComponent<AudioSource>();
            // _playbackAudioSource.pitch = _pitch;
            _playbackAudioSource.clip = null;
            _playbackAudioSource.loop = false;
        }

        uiNewRecording();
    }

    void OnDisable()
    {
        mlAudioBufferClip?.Dispose();
        mlAudioBufferClip = null;

        _playbackAudioSource?.Stop();

        // Stop the time update coroutine
        if (timeUpdateIEnum != null)
        {
            StopCoroutine(timeUpdateIEnum);
        }
    }

    // Callback for the record button
    // Start time update coroutine
    public void StartMicrophone()
    {
        _playbackAudioSource.Stop();
        var captureType = MLAudioInput.MicCaptureType.VoiceCapture;
        var sampleRate = MLAudioInput.GetSampleRate(captureType);
        mlAudioBufferClip = new MLAudioInput.BufferClip(captureType, AUDIO_CLIP_LENGTH_SECONDS, sampleRate);

        recordingImage.sprite = _stopSprite;
        // mlAudioBufferClip.OnReceivedSamples += DetectAudio;

        // Start the time update coroutine
        StartCoroutine((timeUpdateIEnum = updateTime()));
    }

    // Callback for the stop button
    // Flush the clip to audio source
    // Stop time update coroutine
    public void StopCapture()
    {
        // Flush the clip
        // _playbackAudioSource.clip = mlAudioBufferClip.FlushToClip();
        _playbackAudioSource.time = 0;

        // !!! TODO: Only Play for testing. Remove later!!!!!
        _playbackAudioSource.Play();

        // Don't need the buffer clip. Dispose it.
        mlAudioBufferClip?.Dispose();
        mlAudioBufferClip = null;

        // Stop the time update coroutine
        StopCoroutine(timeUpdateIEnum);

        uiConfirmation();
    }

    public void DiscardCallback()
    {
        // UI: Go back to first page
        uiNewRecording();
    }

    public void SaveCallback()
    {
        // TODO: Post _playbackAudioSource.clip to server

        // UI: Go back to first page
        uiNewRecording();
    }

    private void uiConfirmation()
    {
        recordingButton.SetActive(false);
        timeCounterObj.SetActive(false);

        recordingOptionButtons.SetActive(true);
        recordingSystemMsg.SetActive(true);
    }

    private void uiNewRecording()
    {
        recordingPressableButton.ForceSetToggled(false);

        recordingButton.SetActive(true);
        timeCounterObj.SetActive(true);

        recordingOptionButtons.SetActive(false);
        recordingSystemMsg.SetActive(false);

        recordingImage.sprite = _startSprite;
        currentTime = 0f;
        timeCounter.text = "00:00:00";
    }

//    private void DetectAudio(float[] samples)
//    {
//
//    }
}
