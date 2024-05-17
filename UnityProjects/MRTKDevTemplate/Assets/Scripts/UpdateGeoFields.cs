using MixedReality.Toolkit.Suits.Map;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UpdateGeoFields : MonoBehaviour
{

    public TSScConnection tss;


    public TextMeshProUGUI Title;


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

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {


        if (tss.isSPECUpdated())
        {



            RockInfo rockInfo = JsonConvert.DeserializeObject<DeserializeRock>(tss.GetSPECJsonString()).spec;
            Rock toShow = eva1?rockInfo.eva1:rockInfo.eva2;

            if (Title is not null)
            {
                Title.text = toShow.name;
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



        }
    }
}
