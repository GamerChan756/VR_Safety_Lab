using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class LightSwitch : NetworkBehaviour
{
    public bool isPowered = false;

    
    // offline mode exists to ensure the object functions
    // both when connected to the internet and when not connected
    // this way you can potentially use it in a hub world without it causing problems
    private bool offlineMode = true;

    private NetworkVariable<bool> networkedIsPower = new NetworkVariable<bool> ();
    private bool shouldBroadcastChange = false;

    NetworkObject nobj;

    public override void OnNetworkSpawn () {
        offlineMode = false;
        if (IsOwner) {
            networkedIsPower.Value = isPowered;
        }
        else {
            isPowered = networkedIsPower.Value;
        }
        networkedIsPower.OnValueChanged += UpdateIsPowered;
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
        nobj = GetComponent<NetworkObject> ();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldBroadcastChange) {
            if (IsOwner) {
                // broadcasts the light toggle once it's authorized to.
                networkedIsPower.Value = isPowered;
                shouldBroadcastChange = false;
            }
            else {
            }
        }
    }

    private void UpdateIsPowered (bool privous, bool current) {
        isPowered = current;
    }

    public void ToggleLight () {
        isPowered = !isPowered;
        nobj.RequestOwnership ();// requests to be the authoritative copy of the game object
        shouldBroadcastChange = true; // queues the game object to broadcast the light toggle to other users once it's authorized to
    }
}
