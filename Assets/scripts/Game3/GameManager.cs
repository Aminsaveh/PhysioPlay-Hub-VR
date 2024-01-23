using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private enum GameLevel
    {
        Small = 3,
        Medium = 6,
        Large = 9
    }


    [SerializeField] private GameLevel size;
    [SerializeField] private Texture[] baseMaps;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private GameObject[] placeholders;
    [SerializeField] private Renderer quadRenderer;
    [SerializeField] private CartMovement _cartMovement;
    [SerializeField] private GameObject quad;
    [SerializeField] private ObjectController obj;
    [SerializeField] private GameTimerScore _score;

    [SerializeField] private GameObject[] temp;
    
    private bool assignpic = true;
    private int selected;
    private int[] pictures;
    
    
    
    private void Start()
    {
        obj.enabled = false;
        int arraySize = (int)size;
        temp = new GameObject[arraySize];
        quad.SetActive(false);
        pictures = RandGenerator(9);
        selected = 0;
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
        
        // Randomly pick a texture from the array
        Texture selectedTexture = baseMaps[pictures[selected]];
                
        // Apply the texture to the Quad's material
        quadRenderer.material.mainTexture = selectedTexture;
                
        gameObject.SetActive(true);
                
        quad.SetActive(true);
        
        StartCoroutine(PictureTimer());
        
    }
    
    private int[] RandWithExclude(int maxRange, int excludeNumber) {
        List<int> randomNumbers = new List<int>();

        // Ensure maxRange is at least 8 to find 8 unique numbers
        if (maxRange < 8) {
            throw new ArgumentException("maxRange must be at least 8.");
        }

        while (randomNumbers.Count < maxRange - 1) {
            int randomNumber = Random.Range(0, maxRange);

            // Check if the number is not the excluded number and not already in the list
            if (randomNumber != excludeNumber && !randomNumbers.Contains(randomNumber)) {
                randomNumbers.Add(randomNumber);
            }
        }

        return randomNumbers.ToArray();
    }
    private int[] RandGenerator(int maxRange) {
        List<int> randomNumbers = new List<int>();
        

        while (randomNumbers.Count < maxRange) {
            int randomNumber = Random.Range(0, maxRange);

            // Check if the number is not the excluded number and not already in the list
            if (!randomNumbers.Contains(randomNumber)) {
                randomNumbers.Add(randomNumber);
            }
        }

        return randomNumbers.ToArray();
    }


    private IEnumerator PictureTimer()
    {
        yield return new WaitForSeconds(5);
        quad.SetActive(false);
        obj.enabled = true;
        ObjectAssign();
    }

    private void ObjectAssign()
    {
        temp[0] = Instantiate(objects[pictures[selected]]);
        int[] eight = RandWithExclude(9, pictures[selected]);
        for (int i = 1; i <temp.Length ; i++)
        {
            temp[i] = Instantiate(objects[eight[i - 1]]);

        }
        PlaceHolderAssign();
    }

    private void PlaceHolderAssign()
    {
        int rand = Random.Range(0, 100);
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].transform.SetParent(placeholders[(rand + i) % (int)size].transform);
            
            // Set the local position of the child to zero to match the parent's position
            temp[i].transform.localPosition = Vector3.zero;

            // Optionally, if you also want the child to have the same rotation and scale as the parent
            temp[i].transform.localRotation = Quaternion.identity;
        }
    }

    internal async void CheckSelected()
    {
            int selectedobject = Int32.MinValue;
            obj.enabled = false;
            
            for (int i = 0; i <temp.Length ; i++)
            {
                if (!temp[i].CompareTag(obj.Tag))
                {
                    Destroy(temp[i]);
                }
                else
                {
                    selectedobject = i;
                }
            }

            await Task.Delay(1000);
            
            if (obj.Number == pictures[selected])
            {
                _score.ScoreCounter();
            }
            
            Destroy(temp[selectedobject]);
            
            
            obj.Number = Int32.MaxValue - 1;
            obj.Tag = "Hello";
            // tabeye scroe ro inja seda bezan
            
            selected++;

            if (selected < baseMaps.Length)
            {
                assignpic = true;
            }
            else
            {
                _score.StopStopwatch();
                await Task.Delay(1000);

                _cartMovement.Resume = true;
                
            }

            

    }

}


