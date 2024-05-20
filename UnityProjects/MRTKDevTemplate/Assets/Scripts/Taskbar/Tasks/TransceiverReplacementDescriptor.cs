using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransceiverReplacementDescriptor : TaskDescriptor
{
    private string myTaskName = "transceiver";
    private string[] myTaskHeadings = {
        // step 1
        "EV1",
        "EV1",
        "EV1",
        "EV1",
        "EV1"
    };

    private string[] myTaskTitles = {
        // step 1
        "INSP",
        // step 2
        "COMM",
        "",
        "",
        // step3
        "Confirm"
    };

    private string[][] myTaskSteps = {
        new string[] {
            "",
        },
        new string[] {
            // step 2 
            "1. Collect replacement transceiver module and non-conductive tools"
        },
        new string[] {
            // step 2 part 1
            "1. Remove the faulty module"
        },
        new string[] {
            // step 2 part 2
            "1. Install the new module ensuring all connections are secure"
        },
        new string[] {
            // step 3 
            "1.  Click NEXT when verification on all ends are complete"
        },
    };

    private UIStateManager uiStateManager;
    void Start()
    {
        uiStateManager = GameObject.Find("UI Controller").GetComponent<UIStateManager>();
    }

    public override string TaskName
    {
        get { return myTaskName; }
    }

    public override string[] TaskHeadings
    {
        get { return myTaskHeadings; }
    }

    public override string[] TaskTitles
    {
        get { return myTaskTitles; }
    }

    public override string[][] TaskSteps
    {
        get { return myTaskSteps; }
    }

    public override bool StepCompleted(int currTask, int currStep, TSScConnection tss)
    {
        return false;
    }

    public override void TaskCompleted()
    {
        Debug.Log("Complete Repair");
        uiStateManager.transitionOutOfRepairUI();
    }
}
