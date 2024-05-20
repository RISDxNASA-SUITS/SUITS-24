using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIATask : MonoBehaviour
{
    [SerializeField]
    private GameObject complete;
    [SerializeField]
    private GameObject inprogress;
    [SerializeField]
    private GameObject incomplete;

    // Start is called before the first frame update
    void Start()
    {
        complete.SetActive(false);
        inprogress.SetActive(false);
        incomplete.SetActive(true);
    }

    public void MarkAsInProgress()
    {
        complete.SetActive(false);
        incomplete.SetActive(false);
        inprogress.SetActive(true);
    }

    public void MarkAsComplete()
    {
        complete.SetActive(true);
        incomplete.SetActive(false);
        inprogress.SetActive(false);
    }
}
