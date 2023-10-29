using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateImage : MonoBehaviour
{

    public Material[] images;

    public GameObject[] cubePlaceHolders;

    public Cube cubePrefab; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Example usage:
        GenerateCubes((int)4);  // Generates the grid based on the selected puzzle size
        
        // Deactivate the cube prefab after instantiation
        cubePrefab.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GenerateCubes(int level)
    {
        int sideLength = Mathf.RoundToInt(Mathf.Sqrt(level));  // For 4 it's 2x2, for 9 it's 3x3, for 16 it's 4x4
        int cubeNumber = 1;

        for (int z = 0; z < sideLength; z++)
        {
            for (int x = 0; x < sideLength; x++)
            {
                Cube cube = Instantiate(cubePrefab, cubePlaceHolders[cubeNumber-1].transform.position, Quaternion.identity);

                cube.index = cubeNumber;
                cube.name = cubeNumber.ToString();  // Naming the cube
                cubeNumber++;

                AdjustUVs(cube.gameObject, x, z, sideLength);
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
