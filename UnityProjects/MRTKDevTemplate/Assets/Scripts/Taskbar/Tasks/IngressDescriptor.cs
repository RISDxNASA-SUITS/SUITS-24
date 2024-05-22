using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IngressDescriptor : TaskDescriptor
{
    private string[] myTaskHeadings = {
        // step 1
        "Connect UIA to DCU",
        // step 2
        "Vent 02 Tanks",
        //step 3
        "Empty Water Tanks",
        // step 4
        "Disconnect UIA from DCU",
    };


    private string[] myTaskTitles = {
        // step 1
        "Connect",
        // step 2
        "O2",
        // step 3
        "Water",
        // step 4
        "Disconnect",
    };


    private string[][] myTaskSteps = {
        new string[] {
            // step 1
            "[UIA & DCU] 1. Connect UIA to the DCU via their umbilical cords",
            "[UIA] Turn  2. EMU PWR on",
            "[BOTH DCU]  3. Connect BATT to UMB"
        },
        new string[] {
            // step 2
            "[UIA]       1. Open OXYGEN O2 VENT",
            "[HMD]       2. Wait until both Primary and Secondary OXY tanks are < 10psi",
            "[UIA]       3. Close OXYGEN O2 VENT"
        },
        new string[] {
            // step 3
            "[BOTH DCU]  1. Open PUMP",
            "[UIA]       2. Open WASTE WATER",
            "[HMD]       3. Wait until water EV1 and EV2 Coolant tank is < 5%",
            "[UIA]       4. Close WASTE WATER"
        },
        new string[] {
            // step 4
            "[UIA & DCU] 1. Turn EMU PWR off",
            "[DCU]       2. Disconnect umbilical"
        },
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
        if (currTask < 0 || currTask >= myTaskHeadings.Length) return false;
        if (currStep < 0 || currStep >= myTaskSteps[currTask].Length) return false;


        UIA uia = tss.GetUIA();
        DCU dcu = tss.GetDCU();


        if (uia == null || dcu == null) return false;


        switch (currTask)
        {
            case 0: // Connect UIA to DCU and start Depress
                switch (currStep)
                {
                    case 0: // (UIA and DCU) EV1 and EV2 connect UIA and DCU umbilical
                        return false;
                    case 1: // (UIA) EV-1, EV-2 PWR - ON
                        return uia.eva1_power && uia.eva2_power;
                    case 2: // (BOTH DCU) BATT - UMB
                        return !dcu.eva1.batt && !dcu.eva2.batt;
                    default:
                        return false;
                }


            case 1: // Vent O2 Tanks
                switch (currStep)
                {
                    case 0: //(UIA) OXYGEN O2 VENT - OPEN 
                        return uia.oxy_vent;
                    case 1: //(UIA) OXYGEN O2 VENT - CLOSE
                        return !uia.oxy_vent;
                    default:
                        return false;
                }



            case 2: // Empty Water Tanks
                switch (currStep)
                {
                    case 0: // (BOTH DCU) PUMP – OPEN
                        return dcu.eva1.pump && dcu.eva2.pump;
                    case 1: // (UIA) EV-1, EV-2 WASTE WATER – OPEN
                        return uia.eva1_water_waste && uia.eva2_water_waste;
                    case 2: // (UIA) EV-1, EV-2 WASTE WATER – CLOSE
                        return !uia.eva1_water_waste && !uia.eva1_water_waste;
                    default:
                        return false;
                }


            case 3: // Disconnect UIA from DCU
                switch (currStep)
                {
                    case 0: // (UIA) EV-1, EV-2 EMU PWR – OFF
                        return !uia.eva1_power && !uia.eva2_power;
                    case 1: // (DCU) EV1 and EV2 disconnect umbilical
                        return false;
                    default:
                        return false;
                }
            default:
                return false;
        }
    }


    public override void TaskCompleted()
    {
        Debug.Log("Complete Ingress");
        uiStateManager.hideCurrentUI();
    }
}