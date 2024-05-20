using Microsoft.MixedReality.OpenXR;
using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.UI;

enum TaskStatus
{
    Complete = 0,
    InProgress,
    Incomplete,
}

public class TaskManager : MonoBehaviour
{
    private UIATask[] tasks;
    private UIATask prevTask = null;
    private int nextTaskIdx = 0;

    [SerializeField]
    private PressableButton nextTaskButton;

    void Start()
    {
        tasks = new UIATask[8]; // Initialize the array with the correct size
        int cnt = 0;
        for (int i = 1; i <= 4; i++)
        {
            int numChildren = i == 1 ? 1 : 2;
            for (int j = 1; j <= numChildren; j++)
            {
                string name = $"Task-List/Task {i}/UIA Task {j}";
                tasks[cnt++] = GameObject.Find(name).GetComponent<UIATask>();
            }
        }
    }

    public void NextTaskCallback()
    {
        if (prevTask != null)
        {
            prevTask.MarkAsComplete();
        }

        if (nextTaskIdx >= tasks.Length)
        {
            return;
        }

        var nextTask = tasks[nextTaskIdx];
        nextTask.MarkAsInProgress();

        nextTaskIdx++;
        prevTask = nextTask;
    }
}
