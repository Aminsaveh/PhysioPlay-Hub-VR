using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartGame : MonoBehaviour
{

    [SerializeField] private Texture[] baseMaps;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private Renderer quadRenderer;
    [SerializeField] private CartMovement _cartMovement;
    [SerializeField] private GameObject quad;
    
    private bool assignpic = true;
    private int selected;

    private void Start()
    {
        quad.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_cartMovement.Target && assignpic)
        {
            if (baseMaps.Length > 0 && quadRenderer != null)
            {
                selected = Random.Range(0, baseMaps.Length);
                // Randomly pick a texture from the array
                Texture selectedTexture = baseMaps[selected];
                print(selectedTexture.name);
                // Apply the texture to the Quad's material
                quadRenderer.material.mainTexture = selectedTexture;
                
                gameObject.SetActive(true);
                
                quad.SetActive(true);
                
            }

            assignpic = false;
        }
    }
}
