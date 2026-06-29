using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class SimpleRepairable : NetworkBehaviour
{
    // publicly exposed varialbes
    [SerializeField] float repairTime = 10;
    [SerializeField] bool resetRepairTimeOnLeave = true;
    [SerializeField] RToolType correctTool = (RToolType)(-1);
    [SerializeField] ModelSwapper swapper;

    // private variables
    private float currentRepairTime = 0;
    private bool isFixed; // used for whether the object is fixed on a given computer
    public bool IsFixed => isFixed;
    private NetworkVariable<bool> netIsFixed = new NetworkVariable<bool> (); // used for syncronizing if its fixed across all players

    // whatever tool is currently being used to repair the model
    // null if nothings being used.
    private RepairTool tool = null;
    private bool offlineMode = true;

    [SerializeField]
    Transform progressIndicator = null;

    [SerializeField]
    Transform progressIndicatorEndPoint = null;
    Vector3 progressIndicatorStartPoint;

    [SerializeField]
    GameTask[] requiredTasks = new GameTask[0];

    [SerializeField]
    GameTask taskInfo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (progressIndicator != null)
            progressIndicatorStartPoint = progressIndicator.position;
        if (swapper == null)
            swapper = GetComponent<ModelSwapper> ();

        if (taskInfo == null)
            taskInfo = GetComponent<GameTask> ();

        SetFixed (false);
    }

    public override void OnNetworkSpawn () {
        offlineMode = false;
        if (IsOwner) { // sets on host end
            netIsFixed.Value = isFixed;
        }
        else { // sets up on client end
            netIsFixed.OnValueChanged += FixedChanged;
        }
        SetFixed (netIsFixed.Value);
        //Debug.Log (IsOwner + " " + IsSessionOwner);

        base.OnNetworkSpawn ();
    }

    public override void OnNetworkDespawn () {
        offlineMode = true;
        if (!IsOwner) {
            netIsFixed.OnValueChanged -= FixedChanged;
        }
        base.OnNetworkDespawn ();
    }

    // Update is called once per frame
    void Update()
    {

        if (IsOwner || offlineMode) // ensures this logic is only run on the host
        {
            // decides if the players have completed enough tasks to make progress on this one
            bool requiredTaskCompletion = true;
            for (int i = 0; i < requiredTasks.Length; i++) {
                if (!requiredTasks[i].TaskCompleted) {
                    requiredTaskCompletion = false;
                    break;
                }
            }
            if (tool != null)
            {// runs if a tool is held up to the object
                if (requiredTaskCompletion)
                    currentRepairTime += Time.deltaTime; // counts down until fully repaired
                
                if (currentRepairTime >= repairTime && !isFixed) { // repairs the object and swaps the model
                                                                   //if (!isFixed)
                                                                   //    SetFixed (true);

                    SetFixed (true);
                    if (!offlineMode)
                        netIsFixed.Value = true; // changes it on all the clients

                }

                // stops repairing the object if the tool is deactivated.
                if (!tool.IsOn) tool = null;
            }
            else if (resetRepairTimeOnLeave && !isFixed) {
                //currentRepairTime = 0; // if its set to reset when the tool leaves the object, then reset it
                currentRepairTime = MathF.Max(currentRepairTime - Time.deltaTime, 0);
            }

            if (progressIndicator != null && progressIndicatorEndPoint) {
                
                progressIndicator.position = Vector3.Lerp (
                    progressIndicatorStartPoint,
                    progressIndicatorEndPoint.position,
                    currentRepairTime / repairTime
                );
            }
        }
    }

    // callback called on clients when the host model swappers object is fixed
    private void FixedChanged (bool previous, bool current) {
        SetFixed (current);
    }

    private void SetFixed (bool isFixed) {
        this.isFixed = isFixed;
        
        // swaps the models
        if (swapper != null)
            swapper.CurrentModel = (isFixed ? 1 : 0);

        if (taskInfo != null)
            taskInfo.TaskCompleted = isFixed;
    }

    private void OnTriggerStay (Collider other) {
        // finds the tool the player is using for repairs
        var newTool = other.GetComponent<RepairTool> ();
        if (newTool != null && tool == null) {
            if ((newTool.ToolType & correctTool) > 0 && newTool.IsOn)
                tool = newTool;
        }
        //Debug.Log (other.name);
    }

    private void OnTriggerExit (Collider other) {
        // detects if the tool leaves the object
        if (tool != null) {
            Debug.Log ($"{tool.name} == {other.gameObject.name} = {tool.gameObject == other.gameObject}");
            if (tool.gameObject == other.gameObject) {
                tool = null;
            }
        }
    }

    
}
