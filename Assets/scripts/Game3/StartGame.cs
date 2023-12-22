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
    [SerializeField] private GameObject[] placeholders;
    [SerializeField] private Renderer quadRenderer;
    [SerializeField] private CartMovement _cartMovement;
    [SerializeField] private GameObject quad;

    private GameObject[] temp = new GameObject[3];
    
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
                
                PictureAssign();

            }

            assignpic = false;
        }
        
    }

    private void PictureAssign()
    {
        selected = Random.Range(0, baseMaps.Length);
        // Randomly pick a texture from the array
        Texture selectedTexture = baseMaps[selected];
                
        // Apply the texture to the Quad's material
        quadRenderer.material.mainTexture = selectedTexture;
                
        gameObject.SetActive(true);
                
        quad.SetActive(true);
        
        StartCoroutine(PictureTimer());
        
    }
    
    private int[] TwoRand(int maxRange, int excludeNumber) {
        int randomNumber1 = 0;
        int randomNumber2 = 0;

        while (randomNumber1 == excludeNumber) {
            randomNumber1 = Random.Range(0, maxRange);
        }

        do {
            randomNumber2 = Random.Range(0, maxRange);
        } while (randomNumber2 == excludeNumber || randomNumber2 == randomNumber1);

        return new int[] { randomNumber1, randomNumber2 };
    }

    private IEnumerator PictureTimer()
    {
        yield return new WaitForSeconds(5);
        quad.SetActive(false);
        ObjectAssign();
    }

    private void ObjectAssign()
    {
        temp[0] = Instantiate(objects[selected]);
        int[] two = TwoRand(6, selected);
        temp[1] = Instantiate(objects[two[0]]);
        temp[2] = Instantiate(objects[two[1]]);
        PlaceHolderAssign();
    }

    private void PlaceHolderAssign()
    {
        int rand = Random.Range(0, 100);
        temp[0].transform.position = placeholders[rand % 3].transform.position;
        temp[1].transform.position = placeholders[(rand + 1) % 3].transform.position;
        temp[2].transform.position = placeholders[(rand + 2) % 3].transform.position;
    }
    
    
}


