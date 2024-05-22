using MixedReality.Toolkit.Suits.Map;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeoSampleController : MonoBehaviour
{
    private int numSamplesScaned = 0;
    private int session = 0;

    private GameObject geoStatus;
    private GameObject geoImageNote;
    private SimpleCamera geoImageNoteCamera;
    private GameObject geoSampleScan;
    private GameObject geoVoiceNote;

    public TSScConnection tssConn;

    public TextMeshProUGUI SiO2;

    public TextMeshProUGUI TiO2;

    public TextMeshProUGUI AL2O3;

    public TextMeshProUGUI FeO;

    public TextMeshProUGUI MnO;

    public TextMeshProUGUI MgO;

    public TextMeshProUGUI CaO;

    public TextMeshProUGUI K2O;

    public TextMeshProUGUI P2O3;

    public TextMeshProUGUI other;
        
    private bool eva1 = true;
    private bool eva2 = false;

    bool firstUpdate = true;
    Rock lastInfo;

    void Start()
    {
        geoStatus = transform.Find("Geo Status").gameObject;
        geoImageNote = transform.Find("Geo Image Note").gameObject;
        geoImageNoteCamera = geoImageNote.GetComponent<SimpleCamera>();
        geoSampleScan = transform.Find("Geo Sample Scan").gameObject;
        geoVoiceNote = transform.Find("Geo Voice Note").gameObject;

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

            geoImageNoteCamera.CaptureCallback();

            lastInfo = toShow;
            firstUpdate = false;
        }

    }
}
