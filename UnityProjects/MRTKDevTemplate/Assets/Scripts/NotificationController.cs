using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    private GameObject systemMsg;
    private TMPro.TMP_Text systemMsgText;

    private GameObject errorWarning;
    private TMPro.TMP_Text errorWarningText;

    private GameObject geoLoadingCompleteMsg;

    private float msgStartTime;
    private float errorStartTime;
    private float msgDuration;
    private float errorDuration;
    private float geoStartTime;
    private float geoDuration;

    void Awake()
    {
        systemMsg = GameObject.Find("System Message");
        systemMsgText = GameObject.Find("System Message Text").GetComponent<TMPro.TMP_Text>();

        errorWarning = GameObject.Find("Error Warning");
        errorWarningText = GameObject.Find("Error Text").GetComponent<TMPro.TMP_Text>();

        geoLoadingCompleteMsg = GameObject.Find("Geo Loading Complete");
    }

    // Start is called before the first frame update
    void Start()
    {
        systemMsg.SetActive(false);
        errorWarning.SetActive(false);
        geoLoadingCompleteMsg.SetActive(false);
    }

    // Update is called once per frame
    void Update() 
    {    
        if (systemMsg.activeSelf)
        {
            float delta = Time.time - msgStartTime;
            if (delta >= msgDuration)
            {
                HideSystemMessage();
            }
        }

        if (errorWarning.activeSelf)
        {
            float delta = Time.time - errorStartTime;
            if (delta >= errorDuration)
            {
                HideErrorWarning();
            }
        }
        
        if (geoLoadingCompleteMsg.activeSelf)
        {
            float delta = Time.time - geoStartTime;
            if (delta >= geoDuration)
            {
                HideGeoComplete();
            }
        }
    }

    public void PushSystemMessage(string msg, float duration)
    {
        msgStartTime = Time.time;
        systemMsgText.text = msg;
        msgDuration = duration;
        systemMsg.SetActive(true);
    }

    public void PushErrorWarning(string err, float duration)
    {
        errorStartTime = Time.time;
        errorWarningText.text = err;
        errorDuration = duration;
        errorWarning.SetActive(true);
    }

    public void PushGeoComplete(float duration)
    {
        geoStartTime = Time.time;
        geoDuration = duration;
        geoLoadingCompleteMsg.SetActive(true);
    }

    public void HideSystemMessage()
    {
        systemMsg.SetActive(false);
    }

    public void HideErrorWarning()
    {
        errorWarning.SetActive(false);
    }

    public void HideGeoComplete()
    {
        geoLoadingCompleteMsg.SetActive(false);
    }
}
