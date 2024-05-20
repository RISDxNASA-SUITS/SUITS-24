using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuralDamageDescriptor : TaskDescriptor
{
    private string myTaskName = "structure";

    private string[] myTaskHeadings = {
        // step 1
        "EV1",
        "EV1",
        "EV1",
        "EV1"
        // step 2

    };

    private string[] myTaskTitles = {
        // step 1
        "INSP",
        // step 2
        "COMM",
        "",
        // step3
        "Confirm"
    };

    private string[][] myTaskSteps = {
        new string[] {
            // step 1 INSP
            "1. Assess tower for visible structural damage",
            "2. If issue is found, click NEXT to relay to LMCC and standby for procedure"
        },
        new string[] {
            // step 2 COMM part 1
            "1. Apply metal patches over holes or tears using welding tools"
        },
        new string[] {
            // step 2 COMM part 2
            "1. Secure larger structural issues with adhesives and temporary supports"
        },
        new string[] {
            // step 3 Confirm 
            "2. Click NEXT when verification on all ends are complete"
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
        Debug.Log("omplete Repair");
        uiStateManager.transitionOutOfRepairUI();
    }
}
