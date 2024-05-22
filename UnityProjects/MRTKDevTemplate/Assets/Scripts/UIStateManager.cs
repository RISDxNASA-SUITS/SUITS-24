using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.UI.BodyUI;

/*enum UIState
{
    Hidden,
    Egress,
    Ingress,
    Nav,
    //ActiveNav,
    NumUIComponents,
}*/

public class UIStateManager : MonoBehaviour
{
    private GameObject navUI;
    private GameObject egressUI;
    private GameObject ingressUI;
    private GameObject geoUI;
    private GameObject repairUI;
    private GameObject handMenuUI;

    // UI object currently in display
    private GameObject currentUI;
    // private UIState state;

    void Start()
    {
        navUI = GameObject.Find("Nav Canvas");
        egressUI = GameObject.Find("Egress Canvas");
        ingressUI = GameObject.Find("Ingress Canvas");
        geoUI = GameObject.Find("Geo Canvas");
        repairUI = GameObject.Find("Repair Canvas");
        handMenuUI = GameObject.Find("Hand Menu");

        navUI.SetActive(false);
        egressUI.SetActive(true);
        ingressUI.SetActive(false);
        geoUI.SetActive(false);
        repairUI.SetActive(false);

        transitionToEgressUI();
    }

    private void transitionToUI(GameObject toUI)
    {
        if (currentUI != toUI)
        {
            currentUI?.SetActive(false);
            currentUI = toUI;
            currentUI?.SetActive(true);
        }
    }

    public void hideCurrentUI()
    {
        transitionToUI(null);
    }

    public void transitionToNavUI()
    {
        transitionToUI(navUI);
    }

    public void transitionToEgressUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(egressUI);
    }

    public void transitionOutOfEgressUI()
    {
        hideCurrentUI();
        handMenuUI.SetActive(true);
    }

    public void transitionToIngressUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(egressUI);
    }


    public void transitionToGeoUI()
    {
        transitionToUI(geoUI);
    }

    public void transitionToRepairUI()
    {
        transitionToUI(repairUI);
    }

    public void transitionOutOfRepairUI()
    {
        repairUI.SetActive(false);
        handMenuUI.SetActive(true);
    }
}
