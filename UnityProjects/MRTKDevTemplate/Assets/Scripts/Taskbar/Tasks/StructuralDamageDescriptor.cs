using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuralDamageDescriptor : TaskDescriptor
{
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
                        return dcu.eva1.batt && dcu.eva2.batt;
                    case 3: // (DCU) EPRESS PUMP PWR - ON
                        return dcu.eva1.pump && dcu.eva2.pump;
                    default:
                        return false;
                }

            case 1: //Prep O2 Tanks
                switch (currStep)
                {
                    case 0: //(UIA) OXYGEN O2 VENT - OPEN 
                        return uia.oxy_vent;
                    case 1: //(UIA) OXYGEN O2 VENT - CLOSE
                        return uia.oxy_vent;
                    case 2: //(BOTH DCU) OXY – PRI
                        return dcu.eva1.oxy && dcu.eva2.oxy;
                    case 3: //(UIA) OXYGEN EMU-1, EMU-2 – OPEN
                        return uia.eva1_oxy && uia.eva2_oxy;
                    case 4: //(UIA) OXYGEN EMU-1, EMU-2 – CLOSE
                        return uia.eva1_oxy && uia.eva2_oxy;
                    case 5:  //(BOTH DCU) OXY – SEC
                        return dcu.eva1.oxy && dcu.eva2.oxy;
                    case 6: //(UIA) OXYGEN EMU-1, EMU-2 – OPEN
                        return uia.eva1_oxy && uia.eva2_oxy;
                    case 7: //(UIA) OXYGEN EMU-1, EMU-2 – CLOSE
                        return uia.eva1_oxy && uia.eva2_oxy;
                    case 8: //(BOTH DCU) OXY – PRI
                        return dcu.eva1.oxy && dcu.eva2.oxy;
                    default:
                        return false;
                }
            
            default:
                return false;
        }
    }
}
