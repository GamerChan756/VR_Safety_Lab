using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RepairTool: NetworkBehaviour {
    private bool offlineMode = true;
    [SerializeField]
    private RToolType toolType = RToolType.None;
    public RToolType ToolType => toolType;

    [SerializeField]
    private ToolActivationType toolActivation = ToolActivationType.Always;

    private Rigidbody rigidbody;


    
    private bool isHeld = false; // if the player is currently holding a grabbable object
    public bool IsHeld => isHeld;
    private NetworkVariable<bool> networkedIsHeld = new NetworkVariable<bool> ();

    private bool isOn = false; // if the player is currently activating a grabbable object
    public bool IsOn => toolActivation == ToolActivationType.Always ? true : isOn;
    private NetworkVariable<bool> networkedIsOn = new NetworkVariable<bool> ();

    //public InputActionManager am = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var grabable = GetComponent<XRGrabInteractable> ();
        // sets up events
        if (grabable) {
            grabable.selectEntered.AddListener(Grabbed);
            grabable.selectExited.AddListener (Dropped);
            grabable.activated.AddListener (Activate);
            grabable.deactivated.AddListener (Deactivate);
        }
        rigidbody = GetComponent<Rigidbody> ();
        
    }

    public override void OnNetworkSpawn () {
        offlineMode = false;
        if (IsOwner) {
            networkedIsHeld.Value = isHeld;
            networkedIsOn.Value = isOn;
        }
        else {
            isHeld = networkedIsHeld.Value;
            isOn = networkedIsOn.Value;
        }
        networkedIsHeld.OnValueChanged = IsHeldChanged;
        networkedIsOn.OnValueChanged = IsOnChanged;
        
        base.OnNetworkSpawn ();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Grabbed (SelectEnterEventArgs args) {

        SetIsHeld (true);
        
    }

    private void Dropped (SelectExitEventArgs args) {
        // enables physics (this this fixes a bug with the object holder)
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;

        SetIsHeld (false);
        if (toolActivation == ToolActivationType.Hold) {
            SetIsOn (false);
        }
    }

    public void Activate (ActivateEventArgs args) {
        //Debug.Log ("ACTIVATED");
        if (toolActivation == ToolActivationType.Toggle)
            SetIsOn (!isOn);// toggles whether the object is on if its set to toggle
        else
            SetIsOn (true); // sets it to be turned on if its set to either 'Hold' or 'Always on'
    }

    public void Deactivate (DeactivateEventArgs args) {
        //Debug.Log ("Deactivate");
        if (toolActivation == ToolActivationType.Hold)
            SetIsOn(false);
    }



    // Syncing is On
    private void IsOnChanged (bool previous, bool current) {
        if (!IsOwner) {
            isOn = current;
        }
    }

    // sets whether the tool is on in the authoratative copy
    [Rpc(SendTo.Owner)]
    private void SetIsOnInOwnerRpc (bool value) {
        isOn = value;
        networkedIsOn.Value = value;
    }

    // sets is on on all instances of this object across the network
    private void SetIsOn (bool value) {
        isOn = value;
        if (!offlineMode) {
            if (IsOwner) {
                networkedIsOn.Value = value;
            }
            else {
                SetIsOnInOwnerRpc (value);
            }
        }
    }

    // Syncs IsHeld
    private void IsHeldChanged (bool previous, bool current) {
        if (!IsOwner) {
            isHeld = current;
        }
    }

    // sets whether the tool is currently being held in the authoritative copy
    [Rpc (SendTo.Owner)]
    private void SetIsHeldInOwnerRpc (bool value) {
        isHeld = value;
        networkedIsHeld.Value = value;
    }

    // set is held in all versions of this object across the network
    private void SetIsHeld (bool value) {
        isHeld = value;
        if (!offlineMode) {
            if (IsOwner) {
                networkedIsHeld.Value = value;
            }
            else {
                SetIsHeldInOwnerRpc (value);
            }
        }
    }

}

public enum ToolActivationType {
    Hold,
    Toggle,
    Always,
}

// as per the rules of flag enums, each member must be a power of 2
// << is a right shift. by defining the numbers using right shifts, you can
// guarentee all members are powers of two. For example 1<<2 = 1*2^2 = 4
// Defining it this way makes querying if a tool is the right one really easy (especially if more than one tool
// can be considered the "Right" one. 
[Flags]
public enum RToolType {
    None = 0,
    Wrench = 1,
    Tape = 1<<1,
    WireCutter = 1<<2,
    ScrewDriver = 1<<3,
    Screw = 1<<4,
    Pipe = 1<<5,

    //Everything = (1<<30)-1,
}