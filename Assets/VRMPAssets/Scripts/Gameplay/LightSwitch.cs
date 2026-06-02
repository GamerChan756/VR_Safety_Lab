using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class LightSwitch : NetworkBehaviour
{
    public bool isOn = false;

    
    // offline mode exists to ensure the object functions
    // both when connected to the internet and when not connected
    // this way you can potentially use it in a hub world without it causing problems
    private bool offlineMode = true;

    private NetworkVariable<bool> networkedIsOn = new NetworkVariable<bool> ();
    private bool shouldBroadcastChange = false;

    NetworkObject nobj;

    public override void OnNetworkSpawn () {
        offlineMode = false;
        if (IsOwner) {
            networkedIsOn.Value = isOn;
        }
        else {
            isOn = networkedIsOn.Value;
        }
        networkedIsOn.OnValueChanged += UpdateIsOn;
        base.OnNetworkSpawn ();
    }

    public override void OnNetworkDespawn () {
        offlineMode = true;

        //if (!IsSessionOwner) {
        //}
        networkedIsOn.OnValueChanged -= UpdateIsOn;
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
                networkedIsOn.Value = isOn;
                shouldBroadcastChange = false;
            }
            else {
            }
        }
    }

    private void UpdateIsOn (bool privous, bool current) {
        isOn = current;
    }

    public void ToggleLight () {
        isOn = !isOn;
        nobj.RequestOwnership ();// requests to be the authoritative copy of the game object
        shouldBroadcastChange = true; // queues the game object to broadcast the light toggle to other users once it's authorized to
    }
}
