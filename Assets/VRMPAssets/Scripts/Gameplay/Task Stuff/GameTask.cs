using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameTask: MonoBehaviour {
    // updates every time the Task is completed, or the requirements to complete it
    // are no longer satusfied
    public UnityEvent<bool> TaskStatusChangedEvent = new UnityEvent<bool> ();
    
    [SerializeField]
    private float gradeValue = 10;
    public float GradeValue => gradeValue;


    private bool taskCompleted = false;
    public bool TaskCompleted {
        get {
            return taskCompleted;
        }

        set {
            // ensures that the taskStatusChanged event won't
            // be called if its assigned its current value
            if (value != taskCompleted) {
                taskCompleted = value;
                TaskStatusChanged (value);
            }
            else
                taskCompleted = value;
        }
    }

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private string taskName = "";
    public string TaskName => taskName;

    /*public override void OnNetworkSpawn () {
        offlineMode = false;
        //taskCompleted.OnValueChanged += TaskStatusChanged;
        base.OnNetworkSpawn ();
        
    }

    public override void OnNetworkDespawn () {
        offlineMode = true;
        base.OnNetworkDespawn ();
    }*/


    private void TaskStatusChanged (bool current) {
        // calls the taskStatusChangedEvent
        TaskStatusChangedEvent.Invoke (current);
        // plays the task completed sound
        if (current && audioSource != null) {
            audioSource.Stop ();
            audioSource.time = 0;
            audioSource.Play ();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start () {
        //taskCompleted.OnValueChanged += TaskStatusChanged;

    }

    // Update is called once per frame
    void Update () {

    }
}
