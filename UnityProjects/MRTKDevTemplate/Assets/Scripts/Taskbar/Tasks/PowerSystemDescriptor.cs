using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSystemDescriptor : TaskDescriptor
{
    private string[] myTaskHeadings = {
        // step 1
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
        // step3
        "Confirm"
    };

    private string[][] myTaskSteps = {
        new string[] {
            "",
        },
        new string[] {
            // step 1 
            "1. Assemble diagnostic kit, spare batteries, solar cells, and electrical repair tools"
        },
        new string[] {
            // step 2
            "1. Perform diagnostics to identify power supply issues"
        },
        new string[] {
            // step 2 part 1
            "1. Replace faulty batteries or solar cells"
        },
        new string[] {
            // step 2 part 2 
            "1. Verify power system functionality post-repair"
        },
        new string[] {
            // step 2 part 3
            "1. Click NEXT when verification on all ends are complete"
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
