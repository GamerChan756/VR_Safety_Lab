using UnityEngine;
using Unity.Netcode;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

        currentGrade = 0; ;

        for (int i = 0; i < tasks.Length; i++) {
            if (tasks[i].TaskCompleted) {
                tasksCompleted++;
                currentGrade += tasks[i].GradeValue;
            }
        }

        /*if (tasksCompleted > currentlyCompleted) {
            audio.Stop ();
            audio.time = 0;
            audio.Play ();
        }*/
        Debug.Log ($"Tasks Completed {tasksCompleted}, current grade {currentGrade} / {maximumGrade}");
    }
}
