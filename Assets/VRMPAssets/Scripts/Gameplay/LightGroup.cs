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

    [SerializeField] LightSwitch circuitBreakerSwitch = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // populates the lights and lightSwitches values if they are set to auto populate
        if (autoPopulateLights) lights = GetComponentsInChildren<Light> ();
        if (autoPopulateSwitches) lightSwitches = GetComponentsInChildren<LightSwitch> ();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (HasPower ()) {
            // tallies the number of light switches currently turned on
            int numOn = 0;
            for (int i = 0; i < lightSwitches.Length; i++) {
                if (lightSwitches[i].isPowered) {
                    numOn++;
                }
            }

            
            // calculates if an odd number of light switches are on
            // this way if multiple light switches are hooked up to a light group
            // switching any of them will toggle the lights
            // (we might not need this but its better to have this functionality just in case)
            powered = (numOn % 2) == 1;
        }
        else { // if the breaker switch is currently turned off, then turn the light group off
            powered = false;
        }

        //sets the lights to be either on or off.
        for (int i = 0; i < lights.Length; i++) {
            lights[i].enabled = powered;
        }
    }

    public bool HasPower () {
        // gets whether the light groups circuit breaker is switched on or off
        // (if the Light Group does not have a circuit breaker switch then default to on)
        if (circuitBreakerSwitch == null) {
            return true;
        }

        return circuitBreakerSwitch.isPowered;
    }
}
