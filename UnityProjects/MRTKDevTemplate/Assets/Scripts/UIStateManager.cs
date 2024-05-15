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
    [Tooltip("The image note game object")]
    [SerializeField]
    private GameObject imageNoteUI;

    [Tooltip("The voice note game object")]
    [SerializeField]
    private GameObject voiceNoteUI;

    [Tooltip("The navigation game object")]
    [SerializeField]
    private GameObject navUI;

    [Tooltip("The UIA game object")]
    [SerializeField]
    private GameObject uiaUI;

    [Tooltip("The hand menu game object")]
    [SerializeField]
    private GameObject handMenuUI;

    // UI object currently in display
    private GameObject currentUI;
    // private UIState state;

    void Start()
    {
        imageNoteUI.SetActive(false);
        voiceNoteUI.SetActive(false);
        navUI.SetActive(false);
        uiaUI.SetActive(false);

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

    public void hideCurrentUI()
    {
        transitionToUI(null);
    }

    public void transitionToImageNoteUI()
    {
        transitionToUI(imageNoteUI);    
    }

    public void transitionToVoiceNoteUI()
    {
        transitionToUI(voiceNoteUI);
    }

    public void transitionToNavUI()
    {
        transitionToUI(navUI);
    }

    public void transitionToEgressUI()
    {
        handMenuUI.SetActive(false);
        transitionToUI(uiaUI);
    }

    public void transitionOutOfEgressUI()
    {
        hideCurrentUI();
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
}
