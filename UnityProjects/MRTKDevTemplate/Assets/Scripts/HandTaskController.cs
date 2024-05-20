using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    private GameObject[] handTaskList;

    void Start()
    {
        handTaskList = new GameObject[8]; // Initialize the array with the correct size
        int cnt = 0;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                string name = $"Task-List/Task{i}/UIA Task Item{j}/Task Status";
                GameObject go = GameObject.Find(name); // Corrected gameObject to GameObject
                handTaskList[cnt] = go;
                cnt++;
            }
        }
    }
}
