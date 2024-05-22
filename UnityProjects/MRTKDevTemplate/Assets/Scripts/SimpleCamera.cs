using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using static LMCCAgent;
using UnityEngine.Networking;

static class GraphicExtension
{
    public static T ChangeAlpha<T>(this T g, float newAlpha)
            where T : Graphic
    {
        var color = g.color;
        color.a = newAlpha;
        g.color = color;
        return g;
    }
}


public class SimpleCamera : MonoBehaviour
{
    [SerializeField, Tooltip("Desired width for the camera capture")]
    private int captureWidth = 1280;
    [SerializeField, Tooltip("Desired height for the camera capture")]
    private int captureHeight = 720;
    [SerializeField, Tooltip("The renderer to show the camera capture on RGB format")]
    private Renderer _screenRendererRGB = null;
    [SerializeField, Tooltip("The animation curve for flash effect")]
    private AnimationCurve flashAnimationCurve;
    [SerializeField, Tooltip("The duration of the system message (seconds)")]
    float systemMsgDurationSecs = 0.5f;

    //The identifier can either target the Main or CV cameras.
    // private MLCamera.Identifier _identifier = MLCamera.Identifier.Main;
    private MLCamera.Identifier _identifier = MLCamera.Identifier.CV;
    private MLCamera _camera;
    //Is true if the camera is ready to be connected.
    private bool _cameraDeviceAvailable;

    private MLCamera.CaptureConfig _captureConfig;

    private Texture2D _videoTextureRgb;
    private Sprite _videoSpriteRgb;

    [SerializeField]
    private Image _videoImage;

    //The camera capture state
    bool _isCapturing = false;
    bool _updateViewFinder = true;

    [SerializeField]
    private GameObject optionButtons;
    [SerializeField]
    private GameObject shutterButton;

    [SerializeField]
    private GameObject systemMsgObj;
    [SerializeField]
    private TextMeshProUGUI systemMsg;

    [SerializeField]
    private GameObject flashObject;
    [SerializeField]
    private Image flashImage;

    [SerializeField]
    private Texture2D _testTexture;

    void Start()
    {
        flashObject.SetActive(false);
        optionButtons.SetActive(false);
        systemMsgObj.SetActive(false);
    }

    void OnEnable()
    {
        //This script assumes that camera permissions were already granted.
        _updateViewFinder = true;
        // _updateViewFinder = false;
        StartCoroutine(EnableMLCamera());
        shutterButton.SetActive(true);
    }

    void OnDisable()
    {
        StopCapture();
        systemMsgObj.SetActive(false);
        optionButtons.SetActive(false);
    }

    private IEnumerator doFlash(float durationSec)
    {
        // loop over durationSec
        for (float t = 0.0f; t <= durationSec; t += Time.deltaTime)
        {
            float fraction = Math.Min(t, durationSec) / durationSec; // [0, 1]
            float alpha = flashAnimationCurve.Evaluate(fraction);

            flashImage.ChangeAlpha(alpha);

            yield return null;
        }
    }

    private IEnumerator flashCoro(float durationSec)
    {
        yield return doFlash(durationSec);
        optionButtons.SetActive(true);
        yield return null;
    }

    public void CaptureCallback()
    {
        shutterButton.SetActive(false);
        flashObject.SetActive(true);

        _updateViewFinder = false;

        StartCoroutine(flashCoro(0.4f));
    }

    public void DiscardCallback()
    {
        systemMsg.text = "Image Discarded!";
        StartCoroutine(DisplaySystemMsgForSeconds(systemMsgDurationSecs));

        optionButtons.SetActive(false);
        shutterButton.SetActive(true);
        _updateViewFinder = true;
    }

    //SaveCallback() for Nav
    public void SaveCallback()
    {
        // We have the last captured image stored as a texture.
        // TODO: POST it here to the LMCC server.
        uploadVideoImageToServer();

        systemMsg.text = "Image Saved!";
        StartCoroutine(DisplaySystemMsgForSeconds(systemMsgDurationSecs));

        optionButtons.SetActive(false);
        shutterButton.SetActive(true);
        _updateViewFinder = true;
    }

    // Get metadata ready for a new ImageNote
    public virtual Note newImageNote()
    {
        var note = new Note();
        note.note_type = "image";
        return note;
    }

    public void PostUpdateState(string jsonPayload)
    {
        string url = "http://localhost:5000";

        IEnumerator UpdateState()
        {
            var request = new UnityWebRequest(url + "/post-sample", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log(bodyRaw);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                // Handle the response from the Flask backend
                Debug.Log("Response: " + responseJson);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        StartCoroutine(UpdateState());
    }

    public void uploadVideoImageToServer()
    {
        try
        {
            Note note = newImageNote();
            note.file_ext = "png";

            // Convert Texture2D to PNG
            note.data = ImageConversion.EncodeToPNG(_videoTextureRgb);
            // note.data = ImageConversion.EncodeToPNG(_testTexture);

            var jsonPayload = JsonConvert.SerializeObject(note);
            // Debug.Log(json);
            // TODO: Post to server
            PostUpdateState(jsonPayload);
        }
        catch (Exception e)
        {
            systemMsg.text = $"Failed to save image: {e.ToString()}";
            StartCoroutine(DisplaySystemMsgForSeconds(systemMsgDurationSecs));
        }
    }


    private IEnumerator DisplaySystemMsgForSeconds(float seconds)
    {
        systemMsgObj.SetActive(true);

        yield return new WaitForSeconds(seconds);

        systemMsgObj.SetActive(false);

        yield return null;
    }

    //Waits for the camera to be ready and then connects to it.
    private IEnumerator EnableMLCamera()
    {
        //Checks the main camera's availability.
        while (!_cameraDeviceAvailable)
        {
            MLResult result = MLCamera.GetDeviceAvailabilityStatus(_identifier, out _cameraDeviceAvailable);

            if (result.IsOk == false || _cameraDeviceAvailable == false)
            {
                // Wait until camera device is available
                yield return new WaitForSeconds(1.0f);
            }
        }
        ConnectCamera();
    }

    private void ConnectCamera()
    {
        //Once the camera is available, we can connect to it.
        if (_cameraDeviceAvailable)
        {
            MLCamera.ConnectContext connectContext = MLCamera.ConnectContext.Create();
            connectContext.CamId = _identifier;
            //MLCamera.Identifier.Main is the only camera that can access the virtual and mixed reality flags
            connectContext.Flags = MLCamera.ConnectFlag.CamOnly;
            // connectContext.EnableVideoStabilization = true;

            _camera = MLCamera.CreateAndConnect(connectContext);
            if (_camera != null)
            {
                Debug.Log("Camera device connected");
                ConfigureCameraInput();
                SetCameraCallbacks();
            }
        }
    }

    private void ConfigureCameraInput()
    {
        //Gets the stream capabilities the selected camera. (Supported capture types, formats and resolutions)
        MLCamera.StreamCapability[] streamCapabilities = MLCamera.GetImageStreamCapabilitiesForCamera(_camera, MLCamera.CaptureType.Video);

        if (streamCapabilities.Length == 0)
            return;

        //Set the default capability stream
        MLCamera.StreamCapability defaultCapability = streamCapabilities[0];

        //Try to get the stream that most closely matches the target width and height
        if (MLCamera.TryGetBestFitStreamCapabilityFromCollection(streamCapabilities, captureWidth, captureHeight,
                MLCamera.CaptureType.Video, out MLCamera.StreamCapability selectedCapability))
        {
            defaultCapability = selectedCapability;
        }

        //Initialize a new capture config.
        _captureConfig = new MLCamera.CaptureConfig();
        //Set RGBA video as the output
        MLCamera.OutputFormat outputFormat = MLCamera.OutputFormat.RGBA_8888;
        //Set the Frame Rate to 30fps
        _captureConfig.CaptureFrameRate = MLCamera.CaptureFrameRate._30FPS;
        //Initialize a camera stream config.
        //The Main Camera can support up to two stream configurations
        _captureConfig.StreamConfigs = new MLCamera.CaptureStreamConfig[1];
        _captureConfig.StreamConfigs[0] = MLCamera.CaptureStreamConfig.Create(
            defaultCapability, outputFormat
        );
        StartVideoCapture();
    }

    private void StartVideoCapture()
    {
        MLResult result = _camera.PrepareCapture(_captureConfig, out MLCamera.Metadata metaData);
        if (result.IsOk)
        {
            //Trigger auto exposure and auto white balance
            _camera.PreCaptureAEAWB();
            //Starts video capture. This call can also be called asynchronously 
            //Images capture uses the CaptureImage function instead.
            result = _camera.CaptureVideoStart();
            _isCapturing = MLResult.DidNativeCallSucceed(result.Result, nameof(_camera.CaptureVideoStart));
            if (_isCapturing)
            {
                Debug.Log("Video capture started!");
            }
            else
            {
                Debug.LogError($"Could not start camera capture. Result : {result}");
            }
        }
    }

    private void StopCapture()
    {
        if (_isCapturing)
        {
            _camera.CaptureVideoStop();
        }

        if (_camera != null)
        {
            _camera.Disconnect();
            _camera.OnRawVideoFrameAvailable -= RawVideoFrameAvailable;
        }

        _isCapturing = false;
    }

    //Assumes that the capture configure was created with a Video CaptureType
    private void SetCameraCallbacks()
    {
        //Provides frames in either YUV/RGBA format depending on the stream configuration
        _camera.OnRawVideoFrameAvailable += RawVideoFrameAvailable;
    }

    void RawVideoFrameAvailable(MLCamera.CameraOutput output, MLCamera.ResultExtras extras, MLCameraBase.Metadata metadataHandle)
    {
        if (output.Format == MLCamera.OutputFormat.RGBA_8888 && _updateViewFinder)
        {
            //Flips the frame vertically so it does not appear upside down.
            MLCamera.FlipFrameVertically(ref output);
            UpdateRGBTexture(ref _videoTextureRgb, output.Planes[0], _screenRendererRGB);
        }
    }

    private void UpdateRGBTexture(ref Texture2D videoTextureRGB, MLCamera.PlaneInfo imagePlane, Renderer renderer)
    {
        if (videoTextureRGB != null &&
            (videoTextureRGB.width != imagePlane.Width || videoTextureRGB.height != imagePlane.Height))
        {
            Destroy(videoTextureRGB);
            videoTextureRGB = null;
        }

        if (videoTextureRGB == null)
        {
            videoTextureRGB = new Texture2D((int)imagePlane.Width, (int)imagePlane.Height, TextureFormat.RGBA32, false);
            videoTextureRGB.filterMode = FilterMode.Bilinear;

            _videoSpriteRgb = Sprite.Create(videoTextureRGB, new Rect(0, 0, videoTextureRGB.width, videoTextureRGB.height), new Vector2(0.5f, 0.5f));
            _videoImage.sprite = _videoSpriteRgb;
        }

        int actualWidth = (int)(imagePlane.Width * imagePlane.PixelStride);

        if (imagePlane.Stride != actualWidth)
        {
            var newTextureChannel = new byte[actualWidth * imagePlane.Height];
            for (int i = 0; i < imagePlane.Height; i++)
            {
                Buffer.BlockCopy(imagePlane.Data, (int)(i * imagePlane.Stride), newTextureChannel, i * actualWidth, actualWidth);
            }
            videoTextureRGB.LoadRawTextureData(newTextureChannel);
        }
        else
        {
            videoTextureRGB.LoadRawTextureData(imagePlane.Data);
        }
        videoTextureRGB.Apply();
    }

    public Texture2D GetTextureRGB()
    {
        return _videoTextureRgb;
    }
}
