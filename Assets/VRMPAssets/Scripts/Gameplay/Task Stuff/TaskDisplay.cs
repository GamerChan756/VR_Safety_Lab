using TMPro;
using UnityEngine;

public class TaskDisplay : MonoBehaviour
{
    public TaskTracker taskTracker;
    TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text> ();
        taskTracker.taskListUpdated.AddListener ((string list) => {
            text.text = "Tasts:\n" + list;
        });
        taskTracker.CountTasksCompleted ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
