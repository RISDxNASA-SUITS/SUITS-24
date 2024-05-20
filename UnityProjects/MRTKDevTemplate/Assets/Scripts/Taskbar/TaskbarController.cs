using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskbarController : MonoBehaviour
{
    private int currTask, currStep;
    private int totalStep = 0;

    private TMPro.TMP_Text taskTitle;
    private Transform progressBarT;
    private Transform taskListParentT;
    private GameObject taskListItemPrefab;
    private GameObject bigProgressDotPrefab;
    private GameObject smallProgressDotPrefab;
    private GameObject connectorPrefab;
    private GameObject[] taskListItems;
    private GameObject[] taskProgressDots;

    [SerializeField]
    private TaskDescriptor taskDescriptor;
    private TSScConnection tssConn;
    private LMCCAgent lMCCAgent;

    // Start is called before the first frame update
    void Start()
    {
        string[] taskHeadings = taskDescriptor.TaskHeadings;
        string[] taskTitles = taskDescriptor.TaskTitles;

        taskTitle = transform.Find("Taskbar Panel").Find("Top").Find("Task Title").gameObject.GetComponent<TMPro.TMP_Text>();
        taskListItemPrefab = Resources.Load<GameObject>("Prefabs/Taskbar/Taskbar Item");
        bigProgressDotPrefab = Resources.Load<GameObject>("Prefabs/Taskbar/Big Task");
        smallProgressDotPrefab = Resources.Load<GameObject>("Prefabs/Taskbar/Small Task");
        connectorPrefab = Resources.Load<GameObject>("Prefabs/Taskbar/Connector");
        taskListParentT = transform.Find("Taskbar Panel").Find("Task List");
        progressBarT = transform.Find("Taskbar Panel").Find("Top").Find("Progress Bar");
        tssConn = GameObject.Find("TSS Agent").GetComponent<TSScConnection>();
        lMCCAgent = GameObject.Find("LMCCAgent").GetComponent<LMCCAgent>();

        taskProgressDots = new GameObject[taskHeadings.Length];
        for (int i = 0; i < taskHeadings.Length; i++)
        {
            if (taskTitles[i] != "") // Use big dots to display tasks with titles
            {
                taskProgressDots[i] = Instantiate(bigProgressDotPrefab, progressBarT);
                taskProgressDots[i].transform.Find("Task Title").gameObject.GetComponent<TMPro.TMP_Text>().text = taskTitles[i];
                taskProgressDots[i].transform.Find("Task Caption").gameObject.SetActive(false);
            }
            else // Use small dots to display tasks without titles
            {
                taskProgressDots[i] = Instantiate(smallProgressDotPrefab, progressBarT);
            }

            taskProgressDots[i].transform.Find("Finished Dot").gameObject.SetActive(false);
            taskProgressDots[i].transform.Find("Filled Dot").gameObject.SetActive(false);

            // Insert connectors in-between
            if (i != taskHeadings.Length - 1) Instantiate(connectorPrefab, progressBarT);
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
        if (taskListItems != null)
        {
            foreach (GameObject obj in taskListItems)
            {
                if (obj != null)
                {
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
        taskProgressDots[currTask].transform.Find("Filled Dot").gameObject.SetActive(true);
        taskProgressDots[currTask].transform.Find("Empty Dot").gameObject.SetActive(false);
        taskListItems[0].transform.Find("Task Status").Find("Inprogress").gameObject.SetActive(true);
    }

    public void NextStep()
    {
        string[] taskHeadings = taskDescriptor.TaskHeadings;
        string[][] taskSteps = taskDescriptor.TaskSteps;

        lMCCAgent.PostUpdateState(taskDescriptor.TaskName, totalStep);

        currStep++;
        totalStep++;
        taskListItems[currStep - 1].transform.Find("Task Status").Find("Inprogress").gameObject.SetActive(false);
        taskListItems[currStep - 1].transform.Find("Task Status").Find("Complete").gameObject.SetActive(true);

        if (currStep < taskSteps[currTask].Length)
        {
            // Perform the next step
            Debug.Log($"Current Step: {currStep}");
            taskListItems[currStep].transform.Find("Task Status").Find("Inprogress").gameObject.SetActive(true);
        }
        else
        {
            // All steps in current task are complete
            taskProgressDots[currTask].transform.Find("Finished Dot").gameObject.SetActive(true);
            currTask++;

            if (currTask < taskHeadings.Length)
            {
                // Transition to next task
                Debug.Log($"Next Task: {currTask}");
                SetupTask();
            }
            else
            {
                // All tasks are complete
                taskDescriptor.TaskCompleted();
            }
        }
    }
}
