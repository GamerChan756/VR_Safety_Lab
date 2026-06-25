using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class MeshMerger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var childMeshes = GetComponentsInChildren<MeshFilter> ();
        var meshRenderer = GetComponent<MeshRenderer> ();
        meshRenderer.materials = childMeshes[0].GetComponent<MeshRenderer> ().materials;
        var instances = new CombineInstance[childMeshes.Length];

        for (int i = 0; i < childMeshes.Length; i++) {
            var combInstance = new CombineInstance () {
                mesh = childMeshes[i].mesh,
                transform = transform.worldToLocalMatrix * childMeshes[i].transform.localToWorldMatrix,
                subMeshIndex = 0
            };
            instances[i] = combInstance;

            Destroy(childMeshes[i].GetComponent<MeshRenderer> ());
            
        }
        var mesh = new Mesh ();
        mesh.CombineMeshes (instances);
        var meshFilter = gameObject.AddComponent<MeshFilter> ();
        meshFilter.mesh = mesh;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
