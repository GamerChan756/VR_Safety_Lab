using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RepairToolPlace : NetworkBehaviour
{


    NetworkVariable<bool> filled = new NetworkVariable<bool> ();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter (Collider collision) {
        var repairTool = collision.GetComponent<RepairTool> ();
        var grabable = collision.GetComponent<XRGrabInteractable> ();
        if (repairTool != null && grabable != null) {
            grabable.enabled = false;
        }
    }
}
