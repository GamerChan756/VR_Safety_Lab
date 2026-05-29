using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ModelSwapper : NetworkBehaviour
{
    [SerializeField] GameObject startModel;
    [SerializeField] GameObject fixedModel;
    [SerializeField] float repairTime = 10;
    [SerializeField] bool resetRepairTimeOnLeave = true;

    float currentRepairTime = 0;
    bool isFixed;

    NetworkVariable<bool> netIsFixed = new NetworkVariable<bool>();

    public GameObject tool = null;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fixedModel.SetActive (false);
        //Debug.Log (IsOwner + " " + IsSessionOwner);
    }

    public override void OnNetworkSpawn () {
        if (IsSessionOwner) {
            netIsFixed.Value = false;
        }
        else {
            netIsFixed.OnValueChanged += FixedChanged;
        }
        Debug.Log (IsOwner + " " + IsSessionOwner);

        base.OnNetworkSpawn ();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSessionOwner) {
            if (tool != null) {
                currentRepairTime += Time.deltaTime;
                if (currentRepairTime >= repairTime && !isFixed) {
                    //if (!isFixed)
                    //    SetFixed (true);
                
                    SetFixed (true);
                    netIsFixed.Value = true;
                
                }
            }
            else if (resetRepairTimeOnLeave) {
                currentRepairTime = 0;
            }
            Debug.Log ("AAAAAAA1!!!");
        }
    }

    private void FixedChanged (bool previous, bool current) {
        SetFixed (current);
    }
    
    

    private void SetFixed (bool isFixed) {
        this.isFixed = isFixed;
        startModel.SetActive (!isFixed);
        fixedModel.SetActive (isFixed);
    }

    private void OnTriggerStay (Collider other) {
        if (other.GetComponent<RepairTool> () != null && tool == null)
        {
            tool = other.gameObject;
        }
        //Debug.Log (other.name);
    }

    private void OnTriggerExit (Collider other) {
        if (tool == other.gameObject) {
            tool = null;
        }
    }
}
