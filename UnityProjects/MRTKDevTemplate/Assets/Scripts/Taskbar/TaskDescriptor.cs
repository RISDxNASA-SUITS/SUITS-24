using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskDescriptor : MonoBehaviour
{
    protected string[] taskHeadings;
    protected string[] taskTitles;
    protected string[][] taskSteps;

    public virtual string[] TaskHeadings
    {
        get { return taskHeadings; }
    }

    public virtual string[] TaskTitles
    {
        get { return taskTitles; }
    }

    public virtual string[][] TaskSteps
    {
        get { return taskSteps; }
    }

    public abstract bool StepCompleted(int currTask, int currStep, TSScConnection tss);
    public abstract void TaskCompleted();
}
