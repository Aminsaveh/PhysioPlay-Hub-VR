using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private CartMovement _cart;

    [SerializeField] private Texture[] baseMaps; // Array to hold your textures
    [SerializeField] private Renderer quadRenderer; // Reference to the Renderer of your Quad

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_cart.Target)
        {
            if (baseMaps.Length > 0 && quadRenderer != null)
            {
                // Randomly pick a texture from the array
                Texture selectedTexture = baseMaps[Random.Range(0, baseMaps.Length)];
            
                // Apply the texture to the Quad's material
                quadRenderer.material.mainTexture = selectedTexture;
            }
        }
    }
}
