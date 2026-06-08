using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ModelSwapper : MonoBehaviour
{
    
    [SerializeField] private int currentModel = 0;
    public int CurrentModel {
        get {return currentModel;}

        set {
            modelObjects[currentModel].SetActive (false); // disables the old model

            modelObjects[value].SetActive (true); // enables the new model
            currentModel = value; // sets the current model index to the new model index
        }
    }

    [SerializeField] private GameObject[] modelObjects = new GameObject[0];


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < modelObjects.Length; i++) {
            modelObjects[i].SetActive (false);
        }
        modelObjects[currentModel].SetActive (true);
        //UseModel (0);
    }

    // gets the length of the model list
    public int GetModelNumber () {
        return modelObjects.Length;
    }
}
