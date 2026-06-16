using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RepairToolHolder : NetworkBehaviour
{

    bool offlineMode = true;
    [SerializeField]
    Transform snapTransform;

    [SerializeField]
    RToolType acceptedTypes = (RToolType)(-1);

    [SerializeField]
    bool lockTool = false;

    [SerializeField]
    GameTask taskInfo;

    

    private NetworkVariable<bool> filled = new NetworkVariable<bool> (); // note this value being accurate is untested
    public bool Filled {
        get {
            if (!offlineMode) {
                return filled.Value;
            }
            else {
                return currentlyHeld != null;
            }
        }
    }


    public RepairTool currentlyHeld = null;
    private XRGrabInteractable heldGrabInteractable = null;

    //public RepairTool CurrentlyHeld => currentlyHeld;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (taskInfo == null)
            taskInfo = GetComponent<GameTask> ();
    }

    public override void OnNetworkSpawn () {
        offlineMode = false;
        if (IsOwner) {
            filled.Value = currentlyHeld != null;
        }
    }

    public override void OnNetworkDespawn () {
        offlineMode = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyHeld != null) {
            if (currentlyHeld.IsHeld) { // if the player grabs it, then the repair tool holder should  let go.
                var rigidBody = currentlyHeld.GetComponent<Rigidbody> ();
                rigidBody.isKinematic = false;
                rigidBody.useGravity = true;
                currentlyHeld = null;
                heldGrabInteractable = null;

                if (IsOwner && !offlineMode) {
                    filled.Value = false; // updates the is filled value, which is important for events and such
                    
                }
            }
            else {
                // continously updates the repair tool incase the repair tool holder moves
                currentlyHeld.transform.position = snapTransform.position;
                currentlyHeld.transform.rotation = snapTransform.rotation;
            }
        }

        if (taskInfo != null && (IsOwner || offlineMode)) {
            taskInfo.TaskCompleted = Filled;
            Debug.Log (Filled);
        }
    }

    

    private void OnTriggerStay (Collider collision) {
        var repairTool = collision.GetComponent<RepairTool> ();
        
        if (repairTool != null && currentlyHeld == null) {
            if (!repairTool.IsHeld) { // ran if the object has the repair tool script, and it is currently not being held
                var grabbable = collision.GetComponent<XRGrabInteractable> ();
                var rigidBody = collision.GetComponent<Rigidbody> ();
                    
                // sets the rigid body to not move, and snaps it into position
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
                rigidBody.linearVelocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
                repairTool.transform.position = snapTransform.position;
                repairTool.transform.rotation = snapTransform.rotation;
                //Debug.Log ("Grabbed");
                if (IsOwner || offlineMode) { // this only needs to be ran if this is the authoritative copy
                    if (!offlineMode)
                        filled.Value = true;
                }

                currentlyHeld = repairTool;
                heldGrabInteractable = currentlyHeld.GetComponent<XRGrabInteractable> ();
                
            }
        }
        //if (repairTool != null && grabable != null) {
        //    grabable.enabled = false;
        //}
    }

    
}
