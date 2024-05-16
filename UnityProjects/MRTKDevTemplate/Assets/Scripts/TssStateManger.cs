using MixedReality.Toolkit.Suits.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine.Networking;


public class TssStateManger : MonoBehaviour
{
    // Start is called before the first frame update
    public string Host = "localhost";
    public int RisdTeamNum = 10;
    public string Port;
    private TSScConnection tss;
    public IEnumerable<Rock> Rocks;
    public UiaData uia;
    private System.Timers.Timer rockTimer;
    private string url;
    private float timeSinceLastUpdate;

    IEnumerator GetRocks()
    {
        Debug.Log("called");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url + "/json_data/rocks/RockData.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log(webRequest.downloadHandler.text);
                    Rocks = JsonConvert.DeserializeObject<RockList>(webRequest.downloadHandler.text).ROCKS;
                    break;
                default:
                    Rocks = null;
                    Debug.Log("failed");
                    break;


            }

        }
    }

    void UpdateRocks()
    {
        StartCoroutine(GetRocks());
        Debug.Log("called");

    }

    void Start()
    {
        tss = gameObject.AddComponent<TSScConnection>();
        tss.ConnectToHost(Host, RisdTeamNum);
        url = "http://" + this.Host + ":" + this.Port;







    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate > 1.0f)
        {
            StartCoroutine(GetRocks());
            timeSinceLastUpdate = 0f;
        }
        if (tss.isUIAUpdated())
        {
            this.uia = new UiaData(tss.GetUIAJsonString());
            Debug.Log(uia.Data);
        }



    }


}
