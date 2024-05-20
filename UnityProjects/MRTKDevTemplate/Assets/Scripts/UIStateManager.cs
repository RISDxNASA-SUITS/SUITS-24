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

    [Tooltip("The geo sampling game object")]
    [SerializeField]
    private GameObject geoSamplingUI;

    [Tooltip("The hand menu game object")]
    [SerializeField]
    private GameObject handMenuUI;

    [Tooltip("The repair game object")]
    [SerializeField]
    private GameObject repairUI;



    // UI object currently in display
    private GameObject currentUI;
    // private UIState state;

    private GameObject testUI;

    void Start()
    {
        navUI.SetActive(false);
        egressUI.SetActive(false);
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

    public void hideCurrentUI(GameObject hideUI)
    {
        // transitionToUI(hideUI);
        hideUI.SetActive(false);
    }

    public void transitionToNavUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(navUI);
    }

    public void transitionOutofNavUI()
    {
        hideCurrentUI(navUI);
        handMenuUI.SetActive(true);
    }

    public void transitionToEgressUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(egressUI);
    }

    public void transitionOutOfEgressUI()
    {
        hideCurrentUI(egressUI);
        handMenuUI.SetActive(true);
    }

    public void transitionToIngressUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(egressUI);
    }

    public void transitionOutOfIngressUI()
    {
        // Do something here?
    }

    public void transitionToGeoSamplingUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(geoSamplingUI);
    }

    public void transitionOutofGeoSamplingUI()
    {
        hideCurrentUI(geoSamplingUI);
        handMenuUI.SetActive(true);
    }

    public void transitionToRepairUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(repairUI);
    }

    public void transitionOutofRepairUI()
    {
        hideCurrentUI(repairUI);
        handMenuUI.SetActive(true);
    }

}
