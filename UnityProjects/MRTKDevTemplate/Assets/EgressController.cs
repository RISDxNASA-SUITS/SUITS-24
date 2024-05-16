using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgressController : MonoBehaviour
{
    private string[] taskHeadings = {
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

    private string[][] taskSteps = {
        new string[] {
            // step 1
            "(UIA and DCU) EV1 and EV2 connect UIA and DCU umbilical",
            "(UIA) EV-1, EV-2 PWR - ON",
            "(BOTH DCU) BATT - UMB",
            "(UIA) EPRESS PUMP PWR - ON"
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
            "(UIA) V-1, EV-2 SUPPLY WATER - OPEN",
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

    private int currTask, currStep;

    private TMPro.TMP_Text taskTitle;
    private Transform taskListParentT;
    private GameObject taskListItemPrefab;
    private GameObject[] taskListItems;
    private GameObject[] taskProgressDots;

    private UIStateManager ui;

    // Start is called before the first frame update
    void Start()
    {
        taskTitle = GameObject.Find("UIA Task Title").GetComponent<TMPro.TMP_Text>();
        taskListItemPrefab = Resources.Load<GameObject>("Prefabs/UIA/UIA Task Item");
        taskListParentT = GameObject.Find("UIA Task List").transform;
        ui = GameObject.Find("UI Controller").GetComponent<UIStateManager>();

        taskProgressDots = new GameObject[taskHeadings.Length];
        for (int i = 0; i < taskHeadings.Length; i++) {
            taskProgressDots[i] = GameObject.Find(taskHeadings[i]);
        }

        SetupTask();
    }

    void SetupTask()
    {
        // Empty the previous task list
        if (taskListItems != null) {
            foreach (GameObject obj in taskListItems) {
                if (obj != null) {
                    Destroy(obj);
                }
            }
        }

        // Populate the current task list
        currStep = 0;
        int numSteps = taskSteps[currTask].Length;
        taskListItems = new GameObject[numSteps];

        taskTitle.text = taskHeadings[currTask];
        for (int i = 0; i < numSteps; i++)
        {
            taskListItems[i] = Instantiate(taskListItemPrefab, taskListParentT);
            var taskListItemText = taskListItems[i].transform.Find("UIA Task Description").GetComponent<TMPro.TMP_Text>();
            taskListItemText.text = taskSteps[currTask][i];
        }

        // Mark the first step as in-progress
        taskListItems[0].transform.Find("UIA Task Status").transform.Find("UIA Status Inprogress").gameObject.SetActive(true);
    }

    public void NextBtnOnClick()
    {
        currStep++;
        taskListItems[currStep - 1].transform.Find("UIA Task Status").transform.Find("UIA Status Inprogress").gameObject.SetActive(false);
        taskListItems[currStep - 1].transform.Find("UIA Task Status").transform.Find("UIA Status Complete").gameObject.SetActive(true);

        if (currStep < taskSteps[currTask].Length) {
            // Perform the next step
            Debug.Log($"Current Step: {currStep}");
            taskListItems[currStep].transform.Find("UIA Task Status").transform.Find("UIA Status Inprogress").gameObject.SetActive(true);
        }
        else
        {
            // All steps in current task are complete
            taskProgressDots[currTask].transform.Find("Filled Dot").gameObject.SetActive(true);
            currTask++;

            if (currTask < taskHeadings.Length) {
                // Transition to next task
                Debug.Log($"Next Task: {currTask}");
                SetupTask();
            }
            else {
                // All tasks are complete
                Debug.Log("Finished tasks");
                ui.transitionOutOfEgressUI();
            }
        }
    }
}
