using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableRepairDescriptor : TaskDescriptor
{
    private string myTaskName = "cable";
    private string[] myTaskHeadings = {
        // step 1
        "EV1",
        "EV1",
        "EV1",
        "EV1",
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
        "",
        "",
        "",
        "",
        // step3
        "Confirm"
    };

    private string[][] myTaskSteps = {
        new string[] {
            //step 1
            "1. Before beginning the repair process, please proceed with inspecting the worksite",
            "2. Scan Comm Tower for possible issues",
            "3. If issue is found, relay to LMCC and standby for procedure"
        },
        new string[] {
            // step 2 
            "1. Direct to Comm Tower screen",
            "2. Select gear icon",
            "3. Select SHUT DOWN"
        },
        new string[] {
            // step 2 part 1
            "1. Verify SHUT DOWN completed to LMCC and EV2"
        },
        new string[] {
            // step 2 part 2
            "1. From tool box, retrieve spare cable"
        },
        new string[] {
            // step 2 part 3 
            "1. Disconnect damaged cable from Comm Tower",
            "2. When completed, notify LMCC and EV2"
        },
        new string[] {
            // step 2 part 4
            "1. Connect new spare cable from Comm Towerr",
            "2. When completed, notify LMCC and EV2"
        },
        new string[] {
            // step 2 part 5 
            "1. Wait for start-up complete from EV2, click NEXT when status received",
            "EV2 STATUS: ..."
        },
        new string[] {
            // step 2 part 6
            "1. Wait for start-up complete from EV2, click DONE when status received",
            "EV2 STATUS: Complete"
        },
        new string[] {
            // step 3
            "1. Direct to Comm Tower screen",
            "2. Verify channel “B” is operational",
            "3. When completed, notify LMCC and EV2"
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
