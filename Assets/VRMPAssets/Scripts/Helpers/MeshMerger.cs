using System;
using UnityEngine;

public class MeshMerger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var meshFilter = GetComponent<MeshFilter> ();
        var meshRenderer = GetComponent<MeshRenderer> ();
        var childMeshes = GetComponentsInChildren<MeshFilter> ();
        var combinedInstances = new CombineInstance[childMeshes.Length - 1];
        int instanceIndex = 0;
        for (int i = 1; i < combinedInstances.Length; i++) {
            if (childMeshes[i].gameObject == gameObject) continue;
            combinedInstances[instanceIndex] = new CombineInstance ();
            combinedInstances[instanceIndex].transform =
                transform.worldToLocalMatrix *
                childMeshes[i].transform.localToWorldMatrix;
            combinedInstances[instanceIndex].mesh = childMeshes[i].mesh;
            var renderer = childMeshes[i-instanceIndex].GetComponent<MeshRenderer> ();
            for (int j = 0; j < meshRenderer.materials.Length; j++) {
                if (renderer.materials[0].ComputeCRC() == meshRenderer.materials[j].ComputeCRC()) {
                    combinedInstances[i-instanceIndex].subMeshIndex = j;
                    Debug.Log ($"{instanceIndex} {j}");
                    break;
                }
            }
            renderer.enabled = false;
            instanceIndex++;
        }
        var mesh = new Mesh ();
        mesh.CombineMeshes (combinedInstances, true);
        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
