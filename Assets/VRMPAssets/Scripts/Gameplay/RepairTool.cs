using System;
using UnityEngine;

public class RepairTool : MonoBehaviour
{
    [SerializeField] RToolType toolType = RToolType.None;
    public RToolType ToolType => toolType;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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