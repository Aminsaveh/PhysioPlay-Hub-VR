using System;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public Material[] materials; // Assign 6 materials in the inspector
    public GameObject cubePrefab; // Assign the Cube prefab with 6 quads

    private void Start()
    {
        if (materials.Length != 6)
        {
            Debug.LogError("Please assign 6 materials.");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject cube = Instantiate(cubePrefab, new Vector3(i * 2, 0, 0), Quaternion.identity);
            AssignCroppedMaterials(cube, i);
        }
    }

    private void AssignCroppedMaterials(GameObject cube, int cropIndex)
    {
        MeshFilter[] quadMeshFilters = cube.GetComponentsInChildren<MeshFilter>();
        
        foreach (MeshFilter meshFilter in quadMeshFilters)
        {
            Mesh mesh = meshFilter.mesh;
            Vector2[] uvs = mesh.uv;

            // Calculate the UV coordinates based on the cropIndex
            float uMin = (cropIndex % 2) * 0.5f;
            float uMax = uMin + 0.5f;
            float vMin = (cropIndex / 2) * 0.5f;
            float vMax = vMin + 0.5f;

            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(
                    Mathf.Lerp(uMin, uMax, uvs[i].x),
                    Mathf.Lerp(vMin, vMax, uvs[i].y)
                );
            }
            
            mesh.uv = uvs;
            meshFilter.mesh = mesh;
            
            // Assign the material to the mesh renderer of the quad
            meshFilter.GetComponent<MeshRenderer>().material = materials[0];
        }
    }
}