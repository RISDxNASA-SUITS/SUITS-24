using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuralDamageDescriptor : TaskDescriptor
{
    private string myTaskName = "structure";

    private string[] myTaskHeadings = {
        // step 1
        "EV2",
        "EV2",
        "EV2",
        "EV2",
        "EV2"
        // step 2

    };

    private string[] myTaskTitles = {
        // step 1
        "Repair",
        "",
        "",
        "",
        // step 2
        "End"
    };

    private string[][] myTaskSteps = {
        new string[] {
            // step 1 INSP
            "1. Assemble safety gear for climbing and securing both crew members.",
        },
        new string[] {
            // step 2 COMM part 1
            "1. Assist EV1 in removing debris and damaged components."
        },
        new string[] {
            // step 2 COMM part 2
            "1. Support EV1 in applying patches or adhesives."
        },
        new string[] {
            // step 3 Confirm 
            "1. Ensure safety protocols are followed during high or difficult access points."
        },
        new string[] {
            //step 4
            "Click NEXT when repair has been confirmed."
        }
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
