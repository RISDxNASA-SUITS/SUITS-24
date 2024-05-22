using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransceiverReplacementDescriptor : TaskDescriptor
{
    private string myTaskName = "transceiver";
    private string[] myTaskHeadings = {
        // step 1
        "EV2",
        "EV2",
        "EV2",
        "EV2",
        "EV2"
    };

    private string[] myTaskTitles = {
        // step 1
        "Repair",
        // step 2
        "",
        // step3
        "End"
    };

    private string[][] myTaskSteps = {
        new string[] {
            "",
        },
        new string[] {
            // step 1 
            "1.   Prepare electrostatic discharge safety equipment."
        },
        new string[] {
            // step 2
            "1.  Assist in isolating power supply."
        },
        new string[] {
            // step 3
            "1.  Ensure module installation is correctly performed; conduct systems check to verify functionality."
        },
        new string[] {
            // step 4
            "1.  Click NEXT when verification on all ends are complete."
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
