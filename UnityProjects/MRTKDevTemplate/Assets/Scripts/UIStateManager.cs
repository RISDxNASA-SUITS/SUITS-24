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

    [Tooltip("The UIA game object")]
    [SerializeField]
    private GameObject uiaUI;

    [Tooltip("The geo sampling game object")]
    [SerializeField]
    private GameObject geoSamplingUI;

    [Tooltip("The hand menu game object")]
    [SerializeField]
    private GameObject handMenuUI;

    

    // UI object currently in display
    private GameObject currentUI;
    // private UIState state;

    private GameObject testUI;

    void Start()
    {
        navUI.SetActive(false);
        uiaUI.SetActive(false);
        geoSamplingUI.SetActive(false);

        transitionToEgressUI();
        
    }

    void Update()
    {

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
        transitionToUI(uiaUI);
    }

    public void transitionOutOfEgressUI()
    {
        hideCurrentUI(uiaUI);
        handMenuUI.SetActive(true);
    }

    public void transitionToIngressUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(uiaUI);
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

}
