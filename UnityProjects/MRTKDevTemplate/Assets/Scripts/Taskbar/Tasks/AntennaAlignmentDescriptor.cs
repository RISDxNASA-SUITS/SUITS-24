using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaAlignmentDescriptor : TaskDescriptor
{
    private string myTaskName = "antenna";
    private string[] myTaskHeadings = {
        // step 1
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
        "",
        // step3
        "End"
    };

    private string[][] myTaskSteps = {
        new string[] {
            // step 1
            "1. Assemble securing gear and safety equipment for both crew members."
        },
        new string[] {
            // step 1 part 1
            "1. Assist EV1 in antenna inspection for misalignment or damage."
        },
        new string[] {
            // step 1 part 2
            "1. Support EV1 in adjustment and calibration process; maintain communication with mission control."
        },
        new string[] {
            // step 2
            "Click NEXT when verification on all ends are complete."
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
