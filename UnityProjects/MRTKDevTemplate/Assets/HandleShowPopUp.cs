using MixedReality.Toolkit.Suits.Map;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleShowPopUp : MonoBehaviour
{
    // Start is called before the first frame update
    public TSScConnection tssConn;

    public GameObject PopUp;

    public Rock oldRock;

    public bool eva1 = true;

    public bool eva2 = false;
    float time_since_last_update;

    void Start()
    {
        tssConn = GameObject.Find("SceneContent/TSS Agent").GetComponent<TSScConnection>();
        PopUp = GameObject.Find("Geo Sample Scan").GetComponent<GameObject>();


    }

    // Update is called once per frame
    void Update()
    {
        time_since_last_update += Time.deltaTime;
        if (time_since_last_update > 1.0f)
        {
            if (tssConn.isSPECUpdated())
            {
                RockInfo rockInfo = JsonConvert.DeserializeObject<DeserializeRock>(tssConn.GetSPECJsonString()).spec;
                Rock toShow = eva1 ? rockInfo.eva1 : rockInfo.eva2;
                oldRock = toShow;
                PopUp.SetActive(true);
            }
            time_since_last_update = 0;
        }

    }
}
