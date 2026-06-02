using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LightGroup : MonoBehaviour
{
    [SerializeField] Light[] lights = new Light[0];
    [SerializeField] LightSwitch[] lightSwitches = new LightSwitch[0];

    // if true it will get all the light or switch components on its children on start
    [SerializeField] bool autoPopulateLights = true;
    [SerializeField] bool autoPopulateSwitches = true;


    private bool powered = false;
    public bool Powered => powered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (autoPopulateLights) lights = GetComponentsInChildren<Light> ();
        if (autoPopulateSwitches) lightSwitches = GetComponentsInChildren<LightSwitch> ();
    }

    // Update is called once per frame
    void Update()
    {
        int numOn = 0;
        for (int i = 0; i < lightSwitches.Length; i++) {
            if (lightSwitches[i].isOn) {
                numOn++;
            }
        }

        // by basing whether the light switch on is odd, we guarentee that fliping a single light switch will toggle the
        // lights, no matter the state of the other switches
        powered = (numOn % 2) == 1;

        for (int i = 0; i < lights.Length; i++) {
            lights[i].enabled = powered;
        }
    }
}
