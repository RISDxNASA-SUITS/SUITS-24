using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskDescriptor : MonoBehaviour
{
    protected string[] taskHeadings;
    protected string[][] taskSteps;

    public virtual string[] TaskHeadings
    {
        get { return taskHeadings; }
    }

    public virtual string[][] TaskSteps
    {
        get { return taskSteps; }
    }

    public abstract bool StepCompleted(int currTask, int currStep, TSScConnection tss);
}
