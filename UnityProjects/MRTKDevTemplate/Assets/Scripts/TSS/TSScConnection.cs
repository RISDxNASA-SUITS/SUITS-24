using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class TSScConnection : MonoBehaviour
{
    // Connection
    string host;
    string port;
    string url;
    int team_number;
    bool connected;
    float time_since_last_update;

    // Database Jsons
    bool UIAUpdated;
    string UIAJsonString;
    bool DCUUpdated;
    string DCUJsonString;
    bool ROVERUpdated;
    string ROVERJsonString;
    bool SPECUpdated;
    string SPECJsonString;
    bool TELEMETRYUpdated;
    string TELEMETRYJsonString;
    bool COMMUpdated;
    string COMMJsonString;
    bool IMUUpdated;
    string IMUJsonString;
    bool ERRORUpdated;
    string ERRORJsonString;

    // Deserialized Jsons
    UIA uia;
    DCU dcu;
    SPEC spec;
    IMU imu;
    ERROR error;

    // Connect to TSSc with a specific team number
    public void ConnectToHost(string host, int team_number)
    {
        this.host = host;
        this.port = "8080";
        this.team_number = team_number;
        this.url = "http://" + this.host + ":" + this.port;
        Debug.Log(this.url);

        // Test Connection
        StartCoroutine(GetRequest(this.url));
    }

    public void DisconnectFromHost()
    {
        this.connected = false;
    }

    // This Function is called when the program begins
    void Start()
    {
        // ConnectToHost("localhost", 9);
        ConnectToHost("172.20.3.97", 9);
    }

    // This Function is called each render frame
    void Update()
    {
        // If you are connected to TSSc
        if (this.connected)
        {
            // Each Second
            time_since_last_update += Time.deltaTime;
            if (time_since_last_update > 1.0f)
            {
                // Pull TSSc Updates
                StartCoroutine(GetUIAState());
                StartCoroutine(GetDCUState());
                StartCoroutine(GetROVERState());
                StartCoroutine(GetSPECState());
                StartCoroutine(GetTELEMETRYState());
                StartCoroutine(GetCOMMState());
                StartCoroutine(GetIMUState());
                StartCoroutine(GetERRORState());
                time_since_last_update = 0.0f;
            }
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    this.connected = true;
                    break;
            }

        }
    }

    ///////////////////////////////////////////// UIA

    IEnumerator GetUIAState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/UIA.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.UIAJsonString != webRequest.downloadHandler.text)
                    {
                        this.UIAUpdated = true;
                        this.UIAJsonString = webRequest.downloadHandler.text;
                        this.uia = JsonConvert.DeserializeObject<UIAWrapper>(this.UIAJsonString).uia;
                    }
                    break;
            }

        }
    }

    public string GetUIAJsonString()
    {
        UIAUpdated = false;
        return this.UIAJsonString;
    }

    public UIA GetUIA()
    {
        UIAUpdated = false;
        return this.uia;
    }

    public bool isUIAUpdated()
    {
        return UIAUpdated;
    }

    ///////////////////////////////////////////// DCU

    IEnumerator GetDCUState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/DCU.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.DCUJsonString != webRequest.downloadHandler.text)
                    {
                        this.DCUJsonString = webRequest.downloadHandler.text;
                        this.dcu = JsonConvert.DeserializeObject<DCUWrapper>(this.DCUJsonString).dcu;
                        this.DCUUpdated = true;
                    }
                    break;
            }

        }
    }

    public string GetDCUJsonString()
    {
        DCUUpdated = false;
        return this.DCUJsonString;
    }

    public DCU GetDCU()
    {
        DCUUpdated = false;
        return this.dcu;
    }

    public bool isDCUUpdated()
    {
        return DCUUpdated;
    }

    ///////////////////////////////////////////// ROVER

    IEnumerator GetROVERState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/ROVER.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.ROVERJsonString != webRequest.downloadHandler.text)
                    {
                        this.ROVERJsonString = webRequest.downloadHandler.text;
                        this.ROVERUpdated = true;
                        // Debug.Log(this.ROVERJsonString);
                    }
                    break;
            }

        }
    }

    public string GetROVERJsonString()
    {
        ROVERUpdated = false;
        return this.ROVERJsonString;
    }

    public bool isROVERUpdated()
    {
        return ROVERUpdated;
    }

    ///////////////////////////////////////////// SPEC

    IEnumerator GetSPECState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/SPEC.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.SPECJsonString != webRequest.downloadHandler.text)
                    {
                        this.SPECJsonString = webRequest.downloadHandler.text;
                        this.spec = JsonConvert.DeserializeObject<SPECWrapper>(this.SPECJsonString).spec;
                        this.SPECUpdated = true;
                    }
                    break;
            }

        }
    }

    public string GetSPECJsonString()
    {
        SPECUpdated = false;
        return this.SPECJsonString;
    }

    public SPEC GetSPEC()
    {
        this.SPECUpdated = false;
        return this.spec;
    }

    public bool isSPECUpdated()
    {
        return SPECUpdated;
    }

    ///////////////////////////////////////////// TELEMETRY

    IEnumerator GetTELEMETRYState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/teams/" + this.team_number + "/TELEMETRY.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.TELEMETRYJsonString != webRequest.downloadHandler.text)
                    {
                        this.TELEMETRYUpdated = true;
                        this.TELEMETRYJsonString = webRequest.downloadHandler.text;
                        // Debug.Log(this.TELEMETRYJsonString);
                    }
                    break;
            }

        }
    }

    public string GetTELEMETRYJsonString()
    {
        TELEMETRYUpdated = false;
        return this.TELEMETRYJsonString;
    }

    public bool isTELEMETRYUpdated()
    {
        return TELEMETRYUpdated;
    }

    ///////////////////////////////////////////// COMM

    IEnumerator GetCOMMState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/COMM.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.COMMJsonString != webRequest.downloadHandler.text)
                    {
                        this.COMMUpdated = true;
                        this.COMMJsonString = webRequest.downloadHandler.text;
                        // Debug.Log(this.COMMJsonString);
                    }
                    break;
            }

        }
    }

    public string GetCOMMJsonString()
    {
        COMMUpdated = false;
        return this.COMMJsonString;
    }

    public bool isCOMMUpdated()
    {
        return COMMUpdated;
    }

    ///////////////////////////////////////////// IMU

    IEnumerator GetIMUState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/IMU.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.IMUJsonString != webRequest.downloadHandler.text)
                    {
                        this.IMUJsonString = webRequest.downloadHandler.text;
                        this.imu = JsonConvert.DeserializeObject<IMUWrapper>(this.IMUJsonString).imu;
                        this.IMUUpdated = true;
                    }
                    break;
            }

        }
    }

    public string GetIMUJsonString()
    {
        IMUUpdated = false;
        return this.IMUJsonString;
    }

    public IMU GetIMU()
    {
        IMUUpdated = false;
        return this.imu;
    }

    public bool isIMUUpdated()
    {
        return IMUUpdated;
    }

    ///////////////////////////////////////////// ERROR

    IEnumerator GetERRORState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/ERROR.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.ERRORJsonString != webRequest.downloadHandler.text)
                    {
                        this.ERRORJsonString = webRequest.downloadHandler.text;
                        this.error = JsonConvert.DeserializeObject<ERRORWrapper>(this.ERRORJsonString).error;
                        this.ERRORUpdated = true;
                    }
                    break;
            }

        }
    }

    public string GetERRORJsonString()
    {
        ERRORUpdated = false;
        return this.ERRORJsonString;
    }

    public ERROR GetERROR()
    {
        ERRORUpdated = false;
        return this.error;
    }

    public bool isERRORUpdated()
    {
        return ERRORUpdated;
    }
}
