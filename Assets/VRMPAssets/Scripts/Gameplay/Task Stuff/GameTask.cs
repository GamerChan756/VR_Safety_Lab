using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameTask: NetworkBehaviour {
    public UnityEvent<bool> TaskStatusChangedEvent = new UnityEvent<bool> ();
    private bool offlineMode = true;
    private NetworkVariable<bool> taskCompleted = new NetworkVariable<bool> (false);
    public bool TaskCompleted {
        get {
            return taskCompleted.Value;
        }

        set {
            if ((IsOwner || offlineMode) && taskCompleted.Value != value) {
                taskCompleted.Value = value;
            }
            else if (!IsOwner) {
                Debug.LogWarning ($"Warning. This client does not own {name}, so you cannot mutate TaskCompleted");
            }
        }
    }
    public override void OnNetworkSpawn () {
        offlineMode = false;
        //taskCompleted.OnValueChanged += TaskStatusChanged;
        base.OnNetworkSpawn ();
        
    }

    public override void OnNetworkDespawn () {
        offlineMode = true;
        base.OnNetworkDespawn ();
    }


    private void TaskStatusChanged (bool previous, bool current) {
        if (previous != current)
            TaskStatusChangedEvent.Invoke (current);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start () {
        taskCompleted.OnValueChanged += TaskStatusChanged;

    }

    // Update is called once per frame
    void Update () {

    }
}
