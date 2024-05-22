using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableRepairDescriptor : TaskDescriptor
{
    private string myTaskName = "cable";
    private string[] myTaskHeadings = {
        // step 1
        "EV2",
        "EV2",
        "EV2",
        "EV2",
        "EV2",
        "EV2",
        "EV2",
        "EV2"

    };

    private string[] myTaskTitles = {
        // step 1
        "INSP",
        // step 2
        "MMRTG",
        "",
        "",
        "",
        "",
        "",
        // step3
        "Completed"
    };

    private string[][] myTaskSteps = {
        new string[] {
            //step 1
            "Upon arrival, notify LMCC2 and begin inspecting the worksite.",
            "Scan MMRTG for possible issues.",
            "If issue is found, notify LMCC2."
        },
        new string[] {
            // step 2 
            "1. Procedure steps will be displayed here, click NEXT to proceed.",
        },
        new string[] {
            // step 2 part 1
            "1. Wait for EV1 to verify Shutdown complete.",
            "2. On MMRTG Panel, move POWER - OFF, notify EV1 and LMCC1."
        },
        new string[] {
            // step 2 part 2
            "1. Navigate to Comm Tower and retrieve one end of power cable.",
            "2. Take appropriate end of cable to MMRTG.",
            "3. Notify EV1 and LMCC1 upon arrival at MMRTG."
        },
        new string[] {
            // step 2 part 3 
            "1. Wait for EV1 to disconnect damaged cable from Comm Tower.",
            "2. Disconnect damaged cable from MMRTG, notify EV1 and LMCC1."
        },
        new string[] {
            // step 2 part 4
            "1. Wait for EV1 to connect new cable from Comm Tower.",
            "2. Connect new cable from MMRTG, notify EV1 and LMCC1."
        },
        new string[] {
            // step 2 part 5 
            "1. Move Power - ON, notify EV1 and LMCC1.",
            "2. Wait for EV1 to move Power - ON and complete start up."
        },
        new string[] {
            // step 2 part 6
            "1. Wait for EV1 to verify channel “B” is operational.",
            "2. On LMCC1 Go, switch to COM-B.",
            "3. Perform Comm check.",
            "If Comm good, continue on COM-B. Else, all switch to COM-A."

        },
        new string[] {
            // step 3
            "Click NEXT when repair has been confirmed."
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
