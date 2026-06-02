using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LightGroup : MonoBehaviour
{
    Light[] lights = new Light[0];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    LightSwitch[] lightSwitches = new LightSwitch[0];
    void Start()
    {
        lights = GetComponentsInChildren<Light> ();
        lightSwitches = GetComponentsInChildren<LightSwitch> ();
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
        bool isOn = (numOn % 2) == 1;

        for (int i = 0; i < lights.Length; i++) {
            lights[i].enabled = isOn;
        }
    }
}
