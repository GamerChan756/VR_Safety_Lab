using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ModelSwapper : NetworkBehaviour
{
    // variables exposed in editor
    [SerializeField] GameObject startModel;
    [SerializeField] GameObject fixedModel;
    [SerializeField] float repairTime = 10;
    [SerializeField] bool resetRepairTimeOnLeave = true;

    // back end variables
    private float currentRepairTime = 0;
    private bool isFixed; // used for whether the object is fixed on a given computer
    private NetworkVariable<bool> netIsFixed = new NetworkVariable<bool>(); // used for syncronizing if its fixed across all players

    // whatever tool is currently being used to repair the model
    // null if nothings being used.
    private GameObject tool = null;
    private bool offlineMode = true;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fixedModel.SetActive (false);
        //Debug.Log (IsOwner + " " + IsSessionOwner);
    }

    public override void OnNetworkSpawn () {
        offlineMode = false;
        if (IsSessionOwner) { // sets on host end
            netIsFixed.Value = false;
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
        if (!IsSessionOwner) {
            netIsFixed.OnValueChanged -= FixedChanged;
        }
        base.OnNetworkDespawn ();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSessionOwner || offlineMode) // ensures this logic is only run on the host
        {
            if (tool != null) {// runs if a tool is held up to the object
                currentRepairTime += Time.deltaTime; // counts down until fully repaired
                if (currentRepairTime >= repairTime && !isFixed) { // repairs the object and swaps the model
                    //if (!isFixed)
                    //    SetFixed (true);
                
                    SetFixed (true); // fixes it on the host
                    if (!offlineMode)
                        netIsFixed.Value = true; // changes it on all the clients
                
                }
            }
            else if (resetRepairTimeOnLeave) {
                currentRepairTime = 0; // if its set to reset when the tool leaves the object, then reset it
            }
        }
    }

    // callback called on clients when the host model swappers object is fixed
    private void FixedChanged (bool previous, bool current) {
        SetFixed (current);
    }
    
    
    // sets whether the object is repaired on a given client
    private void SetFixed (bool isFixed) {
        this.isFixed = isFixed;
        startModel.SetActive (!isFixed);
        fixedModel.SetActive (isFixed);
    }

    private void OnTriggerStay (Collider other) {
        // finds the tool the player is using for repairs
        if (other.GetComponent<RepairTool> () != null && tool == null)
        {
            tool = other.gameObject;
        }
        //Debug.Log (other.name);
    }

    private void OnTriggerExit (Collider other) {
        // detects if the tool leaves the object
        if (tool == other.gameObject) {
            tool = null;
        }
    }
}
