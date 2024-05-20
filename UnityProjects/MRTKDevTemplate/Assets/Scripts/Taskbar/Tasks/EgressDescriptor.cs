using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgressDescriptor : TaskDescriptor
{
    private string myTaskName = "egress";
    private string[] myTaskHeadings = {
        // step 1
        "Connect UIA to DCU and start Depress",
        // step 2
        "Prep O2 Tanks (1)",
        "Prep O2 Tanks (2)",
        "Prep O2 Tanks (3)",
        // step 3
        "Prep Water Tanks (1)",
        "Prep Water Tanks (2)",
        // step 4
        "END Depress, Check Switches and Disconnect (1)",
        "END Depress, Check Switches and Disconnect (2)",
        "END Depress, Check Switches and Disconnect (3)"
    };

    private string[] myTaskTitles = {
        // step 1
        "Connect",
        // step 2
        "O2",
        "",
        "",
        // step 3
        "Water",
        "",
        // step 4
        "Disconnect",
        "",
        ""
    };

    private string[][] myTaskSteps = {
        new string[] {
            // step 1
            "(UIA and DCU) EV1 and EV2 connect UIA and DCU umbilical",
            "(UIA) EV-1, EV-2 PWR - ON",
            "(BOTH DCU) BATT - UMB",
            "(DCU) EPRESS PUMP PWR - ON"
        },
        new string[] {
            // step 2 part 1
            "(UIA) OXYGEN O2 VENT - OPEN",
            "(HMD) Wait until both Primary and Secondary OXY tanks are < 10psi",
            "(UIA) OXYGEN O2 VENT - CLOSE",
            "(BOTH DCU) OXY - PRI"
        },
        new string[] {
            // step 2 part 2
            "(UIA) OXYGEN EMU-1, EMU-2 - OPEN",
            "(HMD) Wait until EV1 and EV2 Primary O2 tanks > 3000 psi",
            "(UIA) OXYGEN EMU-1, EMU-2 - CLOSE",
            "(BOTH DCU) OXY - SEC"
        },
        new string[] {
            // step 2 part 3
            "(UIA) OXYGEN EMU-1, EMU-2 - OPEN",
            "(HMD) Wait until EV1 and EV2 Secondary O2 tanks > 3000 psi",
            "(UIA) OXYGEN EMU-1, EMU-2 - CLOSE",
            "(BOTH DCU) OXY - PRI"
        },
        new string[] {
            // step 3 part 1
            "(BOTH DCU) PUMP - OPEN",
            "(UIA) EV-1, EV-2 WASTE WATER - OPEN",
            "(HMD) Wait until water EV1 and EV2 Coolant tank is < 5%",
            "(UIA) EV-1, EV-2 WASTE WATER - CLOSE"
        },
        new string[] {
            // step 3 part 2
            "(UIA) EV-1, EV-2 SUPPLY WATER - OPEN",
            "(HMD) Wait until water EV1 and EV2 Coolant tank is > 95%",
            "(UIA) EV-1, EV-2 SUPPLY WATER - CLOSE",
            "(BOTH DCU) PUMP - CLOSE"
        },
        new string[] {
            // step 4 part 1
            "(HMD) Wait until SUIT P, O2 P = 4",
            "(UIA) DEPRESS PUMP PWR - OFF",
            "(BOTH DCU) BATT - LOCAL",
            "(UIA) EV-1, EV-2 PWR - OFF"
        },
        new string[] {
            // step 4 part 2
            "(BOTH DCU) Verify OXY - PRI",
            "(BOTH DCU) Verify COMMS - A",
            "(BOTH DCU) Verify FAN - PRI",
            "(BOTH DCU) Verify PUMP - CLOSE"
        },
        new string[] {
            // step 4 part 3
            "(BOTH DCU) Verify CO2 - PRI",
            "(UIA and DCU) EV1 and EV2 disconnect UIA and DCU umbilical"
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
            case 1: // Prep O2 Tanks (1)
                switch (currStep)
                {
                    case 0: // (UIA) OXYGEN O2 VENT - OPEN
                        return uia.oxy_vent;
                    case 1: // (HMD) Wait until both Primary and Secondary OXY tanks are < 10psi
                        return false;
                    case 2: // (UIA) OXYGEN O2 VENT - CLOSE
                        return !uia.oxy_vent;
                    case 3: // (BOTH DCU) OXY - PRI
                        return dcu.eva1.oxy && dcu.eva2.oxy;
                    default:
                        return false;
                }
            case 2: // Prep O2 Tanks (2)
                switch (currStep)
                {
                    case 0: // (UIA) OXYGEN EMU-1, EMU-2 - OPEN
                        return uia.eva1_oxy && uia.eva2_oxy;
                    case 1: // (HMD) Wait until EV1 and EV2 Primary O2 tanks > 3000 psi
                        return false;
                    case 2: // (UIA) OXYGEN EMU-1, EMU-2 - CLOSE
                        return !uia.eva1_oxy && !uia.eva2_oxy;
                    case 3: // (BOTH DCU) OXY - SEC
                        return !dcu.eva1.oxy && !dcu.eva2.oxy;
                    default:
                        return false;
                }
            case 3: // Prep O2 Tanks (3)
                switch (currStep)
                {
                    case 0: // (UIA) OXYGEN EMU-1, EMU-2 - OPEN
                        return uia.eva1_oxy && uia.eva2_oxy;
                    case 1: // (HMD) Wait until EV1 and EV2 Secondary O2 tanks > 3000 psi
                        return false;
                    case 2: // (UIA) OXYGEN EMU-1, EMU-2 - CLOSE
                        return !uia.eva1_oxy && !uia.eva2_oxy;
                    case 3: // (BOTH DCU) OXY - PRI
                        return dcu.eva1.oxy && dcu.eva2.oxy;
                    default:
                        return false;
                }
            case 4: // Prep Water Tanks (1)
                switch (currStep)
                {
                    case 0: // (BOTH DCU) PUMP - OPEN
                        return dcu.eva1.pump && dcu.eva2.pump;
                    case 1: // (UIA) EV-1, EV-2 WASTE WATER - OPEN
                        return uia.eva1_water_waste && uia.eva2_water_waste;
                    case 2: // (HMD) Wait until water EV1 and EV2 Coolant tank is < 5%
                        return false;
                    case 3: // (UIA) EV-1, EV-2 WASTE WATER - CLOSE
                        return !uia.eva1_water_waste && !uia.eva2_water_waste;
                    default:
                        return false;
                }
            case 5: // Prep Water Tanks (2)
                switch (currStep)
                {
                    case 0: // (UIA) EV-1, EV-2 SUPPLY WATER - OPEN
                        return uia.eva1_water_supply && uia.eva2_water_supply;
                    case 1: // (HMD) Wait until water EV1 and EV2 Coolant tank is > 95%
                        return false;
                    case 2: // (UIA) EV-1, EV-2 SUPPLY WATER - CLOSE
                        return !uia.eva1_water_supply && !uia.eva2_water_supply;
                    case 3: // (BOTH DCU) PUMP - CLOSE
                        return !dcu.eva1.pump && !dcu.eva2.pump;
                    default:
                        return false;
                }
            case 6: // END Depress, Check Switches and Disconnect (1)
                switch (currStep)
                {
                    case 0: // (HMD) Wait until SUIT P, O2 P = 4"
                        return false;
                    case 1: // (UIA) DEPRESS PUMP PWR - OFF
                        return !uia.depress;
                    case 2: // (BOTH DCU) BATT - LOCAL
                        return dcu.eva1.batt && dcu.eva2.batt;
                    case 3: // (UIA) EV-1, EV-2 PWR - OFF
                        return !uia.eva1_power && !uia.eva2_power;
                    default:
                        return false;
                }
            case 7: // END Depress, Check Switches and Disconnect (2)
                switch (currStep)
                {
                    case 0: // (BOTH DCU) Verify OXY - PRI
                        return dcu.eva1.oxy && dcu.eva2.oxy;
                    case 1: // (BOTH DCU) Verify COMMS - A
                        return dcu.eva1.comm && dcu.eva2.comm;
                    case 2: // (BOTH DCU) Verify FAN - PRI
                        return dcu.eva1.fan && dcu.eva2.fan;
                    case 3: // (BOTH DCU) Verify PUMP - CLOSE
                        return !dcu.eva1.pump && !dcu.eva2.pump;
                    default:
                        return false;
                }
            case 8: // END Depress, Check Switches and Disconnect (3)
                switch (currStep)
                {
                    case 0: // (BOTH DCU) Verify CO2 - PRI
                        return dcu.eva1.co2 && dcu.eva2.co2;
                    case 1: // (UIA and DCU) EV1 and EV2 disconnect UIA and DCU umbilical
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
        uiStateManager.transitionOutOfEgressUI();
    }
}