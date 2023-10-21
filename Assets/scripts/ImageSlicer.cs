using UnityEngine;

public class ImageSlicer : MonoBehaviour
{
    // The image to slice
    public Texture2D image;

    // The number of pieces to slice the image into (must be a square number)
    public int pieces = 4;

    // The cubes to assign the image pieces to
    public GameObject[] cubes;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the number of pieces is valid
        if (!IsSquare(pieces))
        {
            Debug.LogError("The number of pieces must be a square number");
            return;
        }

        // Check if the number of cubes matches the number of pieces
        if (cubes.Length != pieces)
        {
            Debug.LogError("The number of cubes must match the number of pieces");
            return;
        }

        // Calculate the size of each piece
        int pieceSize = Mathf.FloorToInt(Mathf.Sqrt(image.width * image.height / pieces));

        // Loop through the pieces
        for (int i = 0; i < pieces; i++)
        {
            // Calculate the position of the piece in the image
            int x = (int) ((i % Mathf.Sqrt(pieces)) * pieceSize);
            int y = (int) ((i / Mathf.Sqrt(pieces)) * pieceSize);

            // Get the pixel data of the piece
            Color[] pixels = image.GetPixels(x, y, pieceSize, pieceSize);

            // Create a new texture with the pixel data
            Texture2D newTexture = new Texture2D(pieceSize, pieceSize);
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            // Assign the new texture to one of the cube's faces
            cubes[i].GetComponent<MeshRenderer>().material.mainTexture = newTexture;
        }
    }

    // Helper method to check if a number is a square number
    bool IsSquare(int n)
    {
        int root = Mathf.RoundToInt(Mathf.Sqrt(n));
        return root * root == n;
    }
}