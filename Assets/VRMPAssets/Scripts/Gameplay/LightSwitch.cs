using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LightSwitch : NetworkBehaviour
{
    // whether the light switch is currently switched on
    public bool isPowered = false;

    
    // offline mode exists to ensure the object functions
    // both when connected to the internet and when not connected
    // this way you can potentially use it in a hub world without it causing problems
    private bool offlineMode = true;

    // used to syncronize when the switch is flipped across the game.
    private NetworkVariable<bool> networkedIsPower = new NetworkVariable<bool> ();

    // this exists for the visual aspect of the light switch
    [SerializeField] Transform switchJoint = null;
    [SerializeField] Vector3 onRotation = Vector3.zero;
    [SerializeField] Vector3 offRotation = Vector3.zero;
    

    NetworkObject nobj;

    

    public override void OnNetworkSpawn () {
        offlineMode = false;
        // syncs the "is powered" values to the session owner
        if (IsSessionOwner) {
            networkedIsPower.Value = isPowered;
        }
        else {
            isPowered = networkedIsPower.Value;
            UpdateSwitchJoint ();
        }
        networkedIsPower.OnValueChanged += UpdateIsPowered; // this is set up for everyone incase the ownership moves
        base.OnNetworkSpawn ();
    }

    public override void OnNetworkDespawn () {
        offlineMode = true;

        //if (!IsSessionOwner) {
        //}
        networkedIsPower.OnValueChanged -= UpdateIsPowered;
        base.OnNetworkDespawn ();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        var interactable = GetComponent<XRSimpleInteractable> ();
        if (interactable != null) {
            //interactable.activated.AddListener (Activated);
            interactable.selectEntered.AddListener ((args) => { ToggleLight (); });
        }
        nobj = GetComponent<NetworkObject> ();
        UpdateSwitchJoint ();
    }

    // callback for when the network variable is updated
    private void UpdateIsPowered (bool privous, bool current) {
        isPowered = current;
        UpdateSwitchJoint ();
    }

    // callback for when the switch is flipped
    public void ToggleLight () {
        isPowered = !isPowered;
        
        if (!offlineMode) {// disables network behavior if not connected to a room to prevent errors
            if (IsOwner) { // updates everyone if this copy is the owner
                networkedIsPower.Value = isPowered;
            }
            else {
                SetPoweredRpc (isPowered); // if this is a client, then update the owner
            }
        }
        
        
        UpdateSwitchJoint ();
    }

    // updates the owner to change the "isPowered" value
    [Rpc (SendTo.Owner)]
    void SetPoweredRpc (bool isPowered) {
        this.isPowered = isPowered; // changes "isPowered" on the owner
        networkedIsPower.Value = isPowered; // changes "isPowered" on everything else
    }

    // updates whether the switch is shown to be on or off to match the actual state of the light switch
    // this goes just about anywhere "isPowered" is written to
    private void UpdateSwitchJoint () {
        if (switchJoint != null) {
            switchJoint.rotation = Quaternion.Euler (isPowered ? onRotation : offRotation);
        }
    }

}
