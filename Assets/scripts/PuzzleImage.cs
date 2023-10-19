using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleImage : MonoBehaviour {

    public Texture2D image; // The image to be used for the puzzle
    public int rows; // The number of rows of cubes
    public int columns; // The number of columns of cubes
    public GameObject cubePrefab; // The prefab of the cube

    void Start () {
        // Calculate the width and height of each piece
        int pieceWidth = image.width / columns;
        int pieceHeight = image.height / rows;

        // Loop over the rows and columns
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                // Create a new cube from the prefab
                GameObject cube = Instantiate(cubePrefab, new Vector3(j, i, 0), Quaternion.identity) as GameObject;

                // Get the sprite renderer component of the cube
                SpriteRenderer spriteRenderer = cube.GetComponent<SpriteRenderer>();

                // Create a new sprite from a part of the image
                Sprite sprite = Sprite.Create(image, new Rect(j * pieceWidth, i * pieceHeight, pieceWidth, pieceHeight), new Vector2(0.5f, 0.5f));

                // Assign the sprite to the cube's material
                spriteRenderer.sprite = sprite;
            }
        }
    }

}
