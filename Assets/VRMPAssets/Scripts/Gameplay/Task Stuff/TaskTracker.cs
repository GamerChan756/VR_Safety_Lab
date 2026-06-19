using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Events;

public class TaskTracker : NetworkBehaviour
{

    //[SerializeField]
    //AudioSource audio;
    [SerializeField]
    private GameTask[] tasks = new GameTask[0];
    public int NumTasks => tasks.Length;


    private int tasksCompleted = 0;
    public int TasksCompleted => tasksCompleted;

    private float currentGrade = 0;
    public float CurrentGrade => currentGrade;

    private float maximumGrade = 0;
    public float MaximumGrade => maximumGrade;

    private string listOfTasks = "";
    public string ListOfTasks => listOfTasks;

    public UnityEvent<string> taskListUpdated = new UnityEvent<string> ();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start ()
    {
        for (int i = 0; i < tasks.Length; i++) {
            tasks[i].TaskStatusChangedEvent.AddListener ((a) => { CountTasksCompleted (); });
            maximumGrade += tasks[i].GradeValue;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CountTasksCompleted () {
        //var currentlyCompleted = tasksCompleted;
        tasksCompleted = 0;

        currentGrade = 0;
        var taskList = new List<(string name, int count, int totalCount)> ();

        for (int i = 0; i < tasks.Length; i++) {
            if (tasks[i].TaskCompleted) {
                tasksCompleted++;
                currentGrade += tasks[i].GradeValue;
            }
            
            bool hasSimilarName = false;
            for (int j = 0; j < taskList.Count; j++) {
                if (tasks[i].TaskName == taskList[j].name) {
                    hasSimilarName = true;
                    //if (!tasks[i].TaskCompleted)
                    taskList[j] = (
                        taskList[j].name,
                        taskList[j].count+(tasks[i].TaskCompleted? 1: 0),
                        taskList[j].totalCount+1
                    ); 
                    break;
                }
            }
            if (!hasSimilarName) {
                taskList.Add ((tasks[i].TaskName, tasks[i].TaskCompleted? 1: 0, 1));
            }
            
        }
        listOfTasks = "";
        for (int i = 0; i < taskList.Count; i++) {
            listOfTasks += $"{taskList[i].name} ({taskList[i].count} / {taskList[i].totalCount} done)\n";
        }

        /*if (tasksCompleted > currentlyCompleted) {
            audio.Stop ();
            audio.time = 0;
            audio.Play ();
        }*/
        Debug.Log ($"Tasks Completed {tasksCompleted}, current grade {currentGrade} / {maximumGrade}");
        Debug.Log ("TaskList: \n" + listOfTasks);
        taskListUpdated.Invoke (listOfTasks);
    }
}
