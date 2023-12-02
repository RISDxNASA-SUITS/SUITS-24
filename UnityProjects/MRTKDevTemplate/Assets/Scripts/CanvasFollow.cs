using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasFollow : MonoBehaviour
{
    // Canvas follow
    [SerializeField] private float distanceFromUser = 0.5f;
    private Transform _canvasTf;
    private Camera _mainCamera;


    // Compass related
    [SerializeField] private float _compassOffsetUp = 0.03f;
    private RawImage _compassImage;
    private TextMeshProUGUI _compassDirText;
    private Transform _compassTF;
    private MeshRenderer _compassRenderer;
    private Vector3 _compassScale;

    float calculateCompassOffsetUp(float angleUp) {
        return _compassOffsetUp + Mathf.Max(10.0f - angleUp, 0.0f) * 0.01f;
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvasTf = GetComponent<RectTransform>().transform;
        _mainCamera = Camera.main;

        _compassImage = GameObject.Find("Compass Image").GetComponent<RawImage>();
        _compassTF = GameObject.Find("Compass").GetComponent<Transform>();
        _compassRenderer = GameObject.Find("Compass").GetComponent<MeshRenderer>();
        _compassScale = _compassTF.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        CanvasDoFollow();
        CompassDoFollow();
    }

    private void CanvasDoFollow()
    {
        Transform cameraTf = _mainCamera.transform;
        Vector3 userPos = cameraTf.position;
        Vector3 userLook = cameraTf.forward;

        userLook.y = 0;
        userLook = Vector3.Normalize(userLook);

        _canvasTf.position = userPos + distanceFromUser * userLook - cameraTf.up * 0.02f;
        _canvasTf.rotation = cameraTf.rotation;
    }

    private void CompassDoFollow()
    {
        // compass angle
        float angle = _mainCamera.transform.localEulerAngles.y;
        _compassImage.uvRect = new Rect(angle / 360.0f, 0.0f, 1.0f, 1.0f);
        // _compassDirText.text = Mathf.RoundToInt(angle).ToString();

        // vert angle
        float angleUp = 360.0f - _mainCamera.transform.localEulerAngles.x;
        if (angleUp > 90.0f)
        {
            _compassTF.localScale = new Vector3(0, 0, 0);
            return;
        }

        // else: normal scale
        _compassTF.localScale = _compassScale;

        Vector3 userLook = _mainCamera.transform.forward;
        Vector3 userUp = _mainCamera.transform.up;
        _compassTF.position = (_mainCamera.transform.position +
                               userLook * distanceFromUser +
                               userUp * calculateCompassOffsetUp(angleUp));
    }

}
