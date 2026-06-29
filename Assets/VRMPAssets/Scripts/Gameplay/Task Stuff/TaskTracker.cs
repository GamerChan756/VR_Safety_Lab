using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using XRMultiplayer;

public class TaskTracker : MonoBehaviour
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
        // tally up the grade values of each task, and completionStatusChanged events
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
        // reset tasks completed and current grade
        tasksCompleted = 0;
        currentGrade = 0;
        
        // creates a list of catagories of tasks
        var taskList = new List<(string name, int count, int totalCount)> ();

        for (int i = 0; i < tasks.Length; i++) {
            // tallies up the number of tasks currently completed, and the current grade
            if (tasks[i].TaskCompleted) {
                tasksCompleted++;
                currentGrade += tasks[i].GradeValue;
            }
            
            // searches whether a catagory already exists for this task
            bool hasSimilarName = false;
            for (int j = 0; j < taskList.Count; j++) {
                if (tasks[i].TaskName == taskList[j].name) {
                    // if a catagory already exists for this task then edit that catagory
                    hasSimilarName = true;
                    taskList[j] = (
                        taskList[j].name,
                        taskList[j].count+(tasks[i].TaskCompleted? 1: 0),
                        taskList[j].totalCount+1
                    ); 
                    break;
                }
            }

            // If a catagory does not exist then create it
            if (!hasSimilarName) {
                taskList.Add ((tasks[i].TaskName, tasks[i].TaskCompleted? 1: 0, 1));
            }
            
        }

        // compliles the catagories into a string
        listOfTasks = "";
        for (int i = 0; i < taskList.Count; i++) {
            listOfTasks += $"{taskList[i].name} ({taskList[i].count} / {taskList[i].totalCount} done)\n";
        }


        //Debug.Log ($"Tasks Completed {tasksCompleted}, current grade {currentGrade} / {maximumGrade}");
        //Debug.Log ("TaskList: \n" + listOfTasks);

        // Part that will impact the Auto Grade part of the Grade UI 

        GameObject autoText = GameObject.Find("Grade Info UI");
        // Sets the textBox to the first AutomaticGrade slot in the server (Normally the professor)
        Transform textBox = autoText.transform.Find("CanvasGroup/Player_List_UI/Viewport/Content/Grading_Slot(Clone)/AutomaticGrade");
        // Sets the currentGrade amount as the class score for the lobby 
        TMP_InputField text = textBox.GetComponent<TMP_InputField>();
        text.text = currentGrade.ToString();

        // updates anything that uses the task lists
        taskListUpdated.Invoke (listOfTasks);
    }
}
