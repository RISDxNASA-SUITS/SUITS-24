using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskbarController : MonoBehaviour
{
    private int currTask, currStep;

    private TMPro.TMP_Text taskTitle;
    private Transform taskListParentT;
    private GameObject taskListItemPrefab;
    private GameObject[] taskListItems;
    private GameObject[] taskProgressDots;
    private UIStateManager uiStateManager;

    [SerializeField]
    private TaskDescriptor taskDescriptor;
    private TSScConnection tssConn;

    // Start is called before the first frame update
    void Start()
    {
        string[] taskHeadings = taskDescriptor.TaskHeadings;

        taskTitle = transform.Find("Taskbar Panel").Find("Top").Find("Task Title").gameObject.GetComponent<TMPro.TMP_Text>();
        taskListItemPrefab = Resources.Load<GameObject>("Prefabs/Taskbar/Taskbar Item");
        taskListParentT = transform.Find("Taskbar Panel").Find("Task List");

        uiStateManager = GameObject.Find("UI Controller").GetComponent<UIStateManager>();
        tssConn = GameObject.Find("TSS Agent").GetComponent<TSScConnection>();
    
        taskProgressDots = new GameObject[taskHeadings.Length];
        for (int i = 0; i < taskHeadings.Length; i++) {
            taskProgressDots[i] = GameObject.Find(taskHeadings[i]);
        }

        SetupTask();
    }

    void Update()
    {
        if (taskDescriptor.StepCompleted(currTask, currStep, tssConn)) NextStep();
    }
    
    void SetupTask()
    {
        string[] taskHeadings = taskDescriptor.TaskHeadings;
        string[][] taskSteps = taskDescriptor.TaskSteps;

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
            var taskListItemText = taskListItems[i].transform.Find("Task Description").GetComponent<TMPro.TMP_Text>();
            taskListItemText.text = taskSteps[currTask][i];
        }

        // Mark the first step as in-progress
        taskListItems[0].transform.Find("Task Status").Find("Inprogress").gameObject.SetActive(true);
    }

    public void NextStep()
    {
        string[] taskHeadings = taskDescriptor.TaskHeadings;
        string[][] taskSteps = taskDescriptor.TaskSteps;

        currStep++;
        taskListItems[currStep - 1].transform.Find("Task Status").Find("Inprogress").gameObject.SetActive(false);
        taskListItems[currStep - 1].transform.Find("Task Status").Find("Complete").gameObject.SetActive(true);

        if (currStep < taskSteps[currTask].Length) {
            // Perform the next step
            Debug.Log($"Current Step: {currStep}");
            taskListItems[currStep].transform.Find("Task Status").Find("Inprogress").gameObject.SetActive(true);
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
                uiStateManager.transitionOutOfEgressUI();
            }
        }
    }
}
