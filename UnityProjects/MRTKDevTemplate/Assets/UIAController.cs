using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAController : MonoBehaviour
{
    private string[] taskHeadings = {
        //step 1
        "Connect UIA to DCU", 
    };

    private List<string>[] taskSteps = {
        new List<string> {
            //step 1
            "A. PLUG: Connect UIA and DCU via the cable",
            "B. SWITCH: UIA EMU POWER-> ON (activates the Umbilical on the UIA side)",
            "C. SWITCH: DCU BATT -> Umbilical (activated the Umbilical on the DCU side)",
            },
    };

    private int currTask, currStep;

    private TMPro.TMP_Text taskTitle;
    private Transform taskListParentT;
    private GameObject taskListItemPrefab;
    private GameObject[] taskListItems;

    void Awake()
    {
        taskTitle = GameObject.Find("UIA Task Title").GetComponent<TMPro.TMP_Text>();
        taskListItemPrefab = Resources.Load<GameObject>("Prefabs/UIA/UIA Task Item");
        taskListParentT = GameObject.Find("UIA Task List").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupTask();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupTask()
    {
        // TODO: Reset task list
        int numSteps = taskSteps[currTask].Count;
        taskListItems = new GameObject[numSteps];

        taskTitle.text = taskHeadings[currTask];
        for (int i = 0; i < numSteps; i++)
        {
            taskListItems[i] = Instantiate(taskListItemPrefab, taskListParentT);
            var taskListItemText = taskListItems[i].transform.Find("UIA Task Description").GetComponent<TMPro.TMP_Text>();
            taskListItemText.text = taskSteps[currTask][i];
        }

        taskListItems[0].transform.Find("UIA Task Status").transform.Find("UIA Status Inprogress").gameObject.SetActive(true);
    }

    public void DoneBtnOnClick()
    {
        // 1. currStep++
        // 2. Bound checking (terminate maybe)
        // 2. Update ListItem Status

        currStep++;
        taskListItems[currStep - 1].transform.Find("UIA Task Status").transform.Find("UIA Status Inprogress").gameObject.SetActive(false);
        taskListItems[currStep - 1].transform.Find("UIA Task Status").transform.Find("UIA Status Complete").gameObject.SetActive(true);

        //Bound Checking
        if(currStep >= taskSteps[currTask].Count) {
            Debug.Log("Finished tasks");
            return;
        }
        Debug.Log($"Current Step: {currStep}");
        taskListItems[currStep].transform.Find("UIA Task Status").transform.Find("UIA Status Inprogress").gameObject.SetActive(true);
    }
}