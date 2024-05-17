using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskDescriptor : MonoBehaviour
{
    protected string[] taskHeadings;
    protected string[][] taskSteps;

    public virtual string[] TaskHeadings
    {
        get { return taskHeadings; }
        set { this.taskHeadings = value; }
    }

    public virtual string[][] TaskSteps
    {
        get { return taskSteps; }
        set { this.taskSteps = value; }
    }
}
