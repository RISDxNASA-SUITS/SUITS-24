using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningController : MonoBehaviour
{
    private GameObject warning;
    private TextMeshProUGUI warningTitle;
    private TextMeshProUGUI warningMessage;
    private bool isShowing = false;
    private float beenShowingFor;
    private float totalDisplayTime = 7f;
    private TSScConnection tSScConnection;

    void Start()
    {
        warning = transform.Find("Warning").gameObject;
        warningTitle = warning.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        warningMessage = warning.transform.Find("Message").gameObject.GetComponent<TextMeshProUGUI>();
        beenShowingFor = 0f;

        tSScConnection = GameObject.Find("TSS Agent").GetComponent<TSScConnection>();
        warning.SetActive(false);
    }

    void Update()
    {
        if (tSScConnection.isERRORUpdated())
        {
            ERROR err = tSScConnection.GetERROR();
            if (err != null)
            {
                string title = "";
                if (err.fan_error) title = "Fan Error";
                else if (err.oxy_error) title = "Oxygen Error";
                else if (err.pump_error) title = "Pump Error";

                if (title != "")
                {
                    ShowWarning(title, "Assess situation with LMCC");
                }
            }
        }

        if (isShowing)
        {
            beenShowingFor += Time.deltaTime;
            if (beenShowingFor > totalDisplayTime)
            {
                HideWarning();
            }
        }
    }

    public void ShowWarning(string title, string message)
    {
        warningTitle.text = title;
        warningMessage.text = message;
        warning.SetActive(true);
        isShowing = true;
    }

    public void HideWarning()
    {
        isShowing = false;
        beenShowingFor = 0;
        warning.SetActive(false);
    }
}
