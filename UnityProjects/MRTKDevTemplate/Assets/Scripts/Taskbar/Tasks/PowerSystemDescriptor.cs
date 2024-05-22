using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSystemDescriptor : TaskDescriptor
{
    private string myTaskName = "power";
    private string[] myTaskHeadings = {
        // step 1
        "EV2",
        "EV2",
        "EV2",
        "EV2",
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
            "1. Carry additional spare parts and protective equipment.",
        },
        new string[] {
            // step 1 
            "1. Assist EV1 in identifying damaged components."
        },
        new string[] {
            // step 2
            "1. Help EV1 in repairing or replacing wiring, ensuring secure connections."
        },
        new string[] {
            // step 2 part 1
            "Click NEXT when verification on all ends are complete."
        }
        
    };

    private UIStateManager uiStateManager;
    void Start()
    {
        uiStateManager = GameObject.Find("UI Controller").GetComponent<UIStateManager>();
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
