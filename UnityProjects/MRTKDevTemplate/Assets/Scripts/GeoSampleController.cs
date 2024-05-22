using MixedReality.Toolkit.Suits.Map;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeoSampleController : MonoBehaviour
{
    private int numSamplesScaned = 0;
    private int session = 0;

    private GameObject geoStatus;

    private GameObject geoImageNote;
    private SimpleCamera geoImageNoteCamera;

    private GameObject geoSampleScan;
    private Texture2D geoSampleScanTexture;
    private Image geoSampleScanImage;

    private float geoSampleScanImageWidth;
    private float geoSampleScanImageHeight;

    private GameObject geoVoiceNote;

    private TSScConnection tssConn;

    private TextMeshProUGUI SiO2;
    private TextMeshProUGUI TiO2;
    private TextMeshProUGUI AL2O3;
    private TextMeshProUGUI FeO;
    private TextMeshProUGUI MnO;
    private TextMeshProUGUI MgO;
    private TextMeshProUGUI CaO;
    private TextMeshProUGUI K2O;
    private TextMeshProUGUI P2O3;
    private TextMeshProUGUI other;

    private bool eva1 = true;
    private bool eva2 = false;

    bool firstUpdate = true;
    Rock lastInfo;

    void Awake()
    {
        tssConn = GameObject.Find("TSS Agent").GetComponent<TSScConnection>();

        geoStatus = transform.Find("Geo Status").gameObject;
        geoImageNote = transform.Find("Geo Image Note").gameObject;
        geoImageNoteCamera = geoImageNote.GetComponent<SimpleCamera>();

        geoSampleScan = transform.Find("Geo Sample Scan").gameObject;
        var geoSampleScanImageObj = geoSampleScan.transform.Find("Image").gameObject;
        geoSampleScanImage = geoSampleScanImageObj.GetComponent<Image>();
        var geoSampleScanImageRT = geoSampleScanImageObj.GetComponent<RectTransform>();

        var w = (int)geoSampleScanImageRT.sizeDelta.x;
        var h = (int)geoSampleScanImageRT.sizeDelta.y;
        geoSampleScanTexture = new Texture2D(w, h, TextureFormat.RGBA32, false);
        geoSampleScanTexture.filterMode = FilterMode.Bilinear;
        geoSampleScanImage.sprite = Sprite.Create(geoSampleScanTexture, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));

        geoVoiceNote = transform.Find("Geo Voice Note").gameObject;
    }

    void updateSampleScanImageTexture()
    {
        Graphics.ConvertTexture(geoImageNoteCamera.GetTextureRGB(), geoSampleScanTexture);
    }

    public void TakeSampleScanImageAndUpdate()
    {
        geoImageNoteCamera.CaptureCallback();
        updateSampleScanImageTexture();
    }

    void OnEnable()
    {
        geoStatus.SetActive(true);
        geoImageNote.SetActive(true);
        geoSampleScan.SetActive(false);
        geoVoiceNote.SetActive(false);
    }

    void Update()
    {
        if (tssConn.isSPECUpdated())
        {
            RockInfo rockInfo = JsonConvert.DeserializeObject<DeserializeRock>(tssConn.GetSPECJsonString()).spec;
            Rock toShow = eva1 ? rockInfo.eva1 : rockInfo.eva2;

            if (toShow == lastInfo && !firstUpdate)
            {
                return;
            }

            SiO2.text = toShow.data.SiO2.ToString("0.0") + "%";
            TiO2.text = toShow.data.TiO2.ToString("0.0") + "%";
            AL2O3.text = toShow.data.Al2O3.ToString("0.0") + "%";
            FeO.text = toShow.data.FeO.ToString("0.0") + "%";
            MnO.text = toShow.data.MnO.ToString("0.0") + "%";
            MgO.text = toShow.data.MgO.ToString("0.0") + "%";
            CaO.text = toShow.data.CaO.ToString("0.0") + "%";
            K2O.text = toShow.data.K2O.ToString("0.0") + "%";
            P2O3.text = toShow.data.P2O3.ToString("0.0") + "%";
            other.text = toShow.data.other.ToString("0.0") + "%";

            TakeSampleScanImageAndUpdate();

            lastInfo = toShow;
            firstUpdate = false;
        }

    }
}
