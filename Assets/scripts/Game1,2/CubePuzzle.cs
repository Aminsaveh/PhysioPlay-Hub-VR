using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;  // Drag your Cube Prefab here in the Inspector
    [SerializeField] private float spacing = 1.1f;   // Define spacing between cubes
    
    private enum PuzzleSize
    {
        Small = 4,
        Medium = 9,
        Large = 16
    }
    
    [SerializeField] private PuzzleSize puzzleSize;  // Select this in the Editor

    void Start()
    {
        // Example usage:
        GenerateCubes((int)puzzleSize);  // Generates the grid based on the selected puzzle size
        
        // Deactivate the cube prefab after instantiation
        cubePrefab.SetActive(false);
        cubePrefab.transform.position = new Vector3(1000, 1000, 1000);
    }

    public void GenerateCubes(int level)
    {
        int sideLength = Mathf.RoundToInt(Mathf.Sqrt(level));  // For 4 it's 2x2, for 9 it's 3x3, for 16 it's 4x4
        int cubeNumber = 1;

        for (int z = 0; z < sideLength; z++)
        {
            for (int x = 0; x < sideLength; x++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                
                cube.name = cubeNumber.ToString();  // Naming the cube
                cubeNumber++;

                AdjustUVs(cube, x, z, sideLength);
            }
        }
    }

    public void AdjustUVs(GameObject cube, int x, int z, int sideLength)
    {
        foreach (MeshFilter meshFilter in cube.GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = meshFilter.mesh;
            Vector2[] uvs = mesh.uv;

            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i].x = (uvs[i].x / sideLength) + (float)x / sideLength;
                uvs[i].y = (uvs[i].y / sideLength) + (float)z / sideLength;
            }

            mesh.uv = uvs;
        }
    }
}
