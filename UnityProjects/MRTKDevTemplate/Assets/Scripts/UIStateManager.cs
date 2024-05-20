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
    [Tooltip("The navigation game object")]
    [SerializeField]
    private GameObject navUI;

    [Tooltip("The egress game object")]
    [SerializeField]
    private GameObject egressUI;

    [Tooltip("The ingress game object")]
    [SerializeField]
    private GameObject ingressUI;

    [Tooltip("The geo sampling game object")]
    [SerializeField]
    private GameObject geoSamplingUI;

    [Tooltip("The repair game object")]
    [SerializeField]
    private GameObject repairUI;

    [Tooltip("The hand menu game object")]
    [SerializeField]
    private GameObject handMenuUI;

    // UI object currently in display
    private GameObject currentUI;
    // private UIState state;

    void Start()
    {
        navUI.SetActive(false);
        egressUI.SetActive(false);
        ingressUI.SetActive(false);
        geoSamplingUI.SetActive(false);
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


    public void transitionToGeoSamplingUI()
    {
        transitionToUI(geoSamplingUI);
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
