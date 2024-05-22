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
    private GeoCamera geoImageNoteCamera;

    private GameObject geoSampleScan;
    private Texture2D geoSampleScanTexture;
    private Image geoSampleScanImage;

    private GameObject geoVoiceNote;

    private TSScConnection tssConn;
    private MapController mapController;

    private TextMeshProUGUI SiO2;
    private TextMeshProUGUI TiO2;
    private TextMeshProUGUI Al2O3;
    private TextMeshProUGUI FeO;
    private TextMeshProUGUI MnO;
    private TextMeshProUGUI MgO;
    private TextMeshProUGUI CaO;
    private TextMeshProUGUI K2O;
    private TextMeshProUGUI P2O3;
    private TextMeshProUGUI other;

    private bool eva1 = false;
    //private bool eva2 = false;

    bool firstUpdate = true;
    Rock lastRock;
    Rock currRock;

    bool isCapturing = false;

    void Awake()
    {
        tssConn = GameObject.Find("TSS Agent").GetComponent<TSScConnection>();
        mapController = GameObject.Find("Map Panel").GetComponent<MapController>();

        geoStatus = transform.Find("Geo Status").gameObject;
        geoImageNote = transform.Find("Geo Image Note").gameObject;
        geoImageNoteCamera = geoImageNote.GetComponent<GeoCamera>();

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

        var geo = gameObject.transform.Find("Geo Sample Scan/GeoSample Wrapper");
        SiO2 = geo.Find("SiO2/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        TiO2 = geo.Find("TiO2/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        Al2O3 = geo.Find("Al2O3/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        FeO = geo.Find("FeO/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        MnO = geo.Find("MnO/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        MgO = geo.Find("MgO/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        CaO = geo.Find("CaO/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        MgO = geo.Find("MgO/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        K2O = geo.Find("K2O/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        P2O3 = geo.Find("P2O3/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
        other = geo.Find("Other/Percentage").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public RockData GetRockData()
    {
        return currRock.data;
    }

    void updateSampleScanImageTexture()
    {
        Graphics.ConvertTexture(geoImageNoteCamera.GetTextureRGB(), geoSampleScanTexture);
    }

    public void CaptureCallback()
    {
        geoImageNoteCamera.CaptureCallback();
    }

    public void SaveCallback()
    {
        geoImageNoteCamera.SaveCallback();
        
        geoImageNote.SetActive(false);

        updateSampleScanImageTexture();
        geoSampleScan.SetActive(true);

        geoVoiceNote.SetActive(false);

        isCapturing = false;
    }

    public void NewGeoSampleCallback()
    {
        // TODO: 
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);

        isCapturing = true;
    }

    void OnEnable()
    {
        geoStatus.SetActive(true);
        geoImageNote.SetActive(true);
        geoSampleScan.SetActive(false);
        geoVoiceNote.SetActive(false);

        isCapturing = true;
    }

    void Update()
    {
        if (tssConn.isSPECUpdated())
        {
            RockInfo rockInfo = JsonConvert.DeserializeObject<DeserializeRock>(tssConn.GetSPECJsonString()).spec;
            currRock = eva1 ? rockInfo.eva1 : rockInfo.eva2;

            if (currRock == lastRock || currRock.id == 0)
            {
                return;
            }

            if (!isCapturing)
            {
                return;
            }

            SiO2.text = currRock.data.SiO2.ToString("0.0") + "%";
            TiO2.text = currRock.data.TiO2.ToString("0.0") + "%";
            Al2O3.text = currRock.data.Al2O3.ToString("0.0") + "%";
            FeO.text = currRock.data.FeO.ToString("0.0") + "%";
            MnO.text = currRock.data.MnO.ToString("0.0") + "%";
            MgO.text = currRock.data.MgO.ToString("0.0") + "%";
            CaO.text = currRock.data.CaO.ToString("0.0") + "%";
            K2O.text = currRock.data.K2O.ToString("0.0") + "%";
            P2O3.text = currRock.data.P2O3.ToString("0.0") + "%";
            other.text = currRock.data.other.ToString("0.0") + "%";

            CaptureCallback();

            lastRock = currRock;
            firstUpdate = false;
        }

    }
}
