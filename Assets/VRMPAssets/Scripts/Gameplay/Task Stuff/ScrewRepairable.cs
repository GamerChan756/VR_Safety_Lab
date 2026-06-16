using UnityEngine;

public class ScrewRepairable : MonoBehaviour
{
    public RepairToolHolder itemHolder;

    public RepairTool currentTool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay (Collider other) {
        if (currentTool == null) {
            var tool = other.GetComponent<RepairTool> ();

            if (itemHolder.Filled && tool != null) {
                currentTool = tool;
            }
        }
    }
}
