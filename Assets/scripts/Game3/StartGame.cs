using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{

    [SerializeField] private Texture[] baseMaps; 
    [SerializeField] private Renderer quadRenderer;
    [SerializeField] private CartMovement _cartMovement;
    
    private bool assignpic = true;

    // Update is called once per frame
    void Update()
    {
        if (_cartMovement.Target && assignpic)
        {
            if (baseMaps.Length > 0 && quadRenderer != null)
            {
                // Randomly pick a texture from the array
                Texture selectedTexture = baseMaps[Random.Range(0, baseMaps.Length)];
            
                // Apply the texture to the Quad's material
                quadRenderer.material.mainTexture = selectedTexture;
            }

            assignpic = false;
        }
    }
}
