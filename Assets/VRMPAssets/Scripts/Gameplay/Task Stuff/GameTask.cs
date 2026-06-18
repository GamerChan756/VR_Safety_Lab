using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameTask: MonoBehaviour {
    public UnityEvent<bool> TaskStatusChangedEvent = new UnityEvent<bool> ();
    
    [SerializeField]
    private float gradeValue = 10;
    public float GradeValue => gradeValue;

    //private bool offlineMode = true;
    private bool taskCompleted = false;

    [SerializeField]
    AudioSource audioSource;
    public bool TaskCompleted {
        get {
            return taskCompleted;
        }

        set {
            if (value != taskCompleted) {
                taskCompleted = value;
                TaskStatusChanged (value);
            }
            else
                taskCompleted = value;
        }
    }
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
        
        TaskStatusChangedEvent.Invoke (current);
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
