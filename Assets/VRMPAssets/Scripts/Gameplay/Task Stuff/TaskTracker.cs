using UnityEngine;
using Unity.Netcode;

public class TaskTracker : NetworkBehaviour
{

    [SerializeField]
    AudioSource audio;
    [SerializeField]
    GameTask[] tasks = new GameTask[0];
    int tasksCompleted = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < tasks.Length; i++) {
            tasks[i].TaskStatusChangedEvent.AddListener ((a) => { CountTasksCompleted (); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CountTasksCompleted () {
        var currentlyCompleted = tasksCompleted;
        tasksCompleted = 0;

        for (int i = 0; i < tasks.Length; i++) {
            if (tasks[i].TaskCompleted) {
                tasksCompleted++;
            }
        }

        if (tasksCompleted > currentlyCompleted) {
            audio.Stop ();
            audio.time = 0;
            audio.Play ();
            Debug.Log ($"Tasks Completed {tasksCompleted}");
        }
    }
}
