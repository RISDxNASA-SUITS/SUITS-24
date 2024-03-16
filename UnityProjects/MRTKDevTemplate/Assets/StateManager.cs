using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private GameObject nav;
    private GameObject uia;


    // Start is called before the first frame update
    void Start()
    {
        nav = GameObject.Find("Nav Panel");
        uia = GameObject.Find("UIA");
        nav.SetActive(false);
        uia.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // void HideUIA()
    // {
    //     uia.SetActive(false);
    // }
}
