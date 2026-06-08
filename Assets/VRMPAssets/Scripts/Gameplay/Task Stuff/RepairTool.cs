using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RepairTool : MonoBehaviour
{
    [SerializeField] RToolType toolType = RToolType.None;
    public RToolType ToolType => toolType;

    public bool isHeld = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var grabable = GetComponent<XRGrabInteractable> ();
        if (grabable) {
            grabable.selectEntered.AddListener(Grabbed);
            grabable.selectExited.AddListener (Dropped);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Grabbed (SelectEnterEventArgs args) {
        isHeld = true;
    }

    public void Dropped (SelectExitEventArgs args) {
        isHeld = false;
    }
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

    //Everything = (1<<30)-1,
}