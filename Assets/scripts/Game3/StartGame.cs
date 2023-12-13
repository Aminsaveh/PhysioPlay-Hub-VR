using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private CartMovement _cart;


    // Start is called before the first frame update
    void Start()
    {
        _cart = GetComponent<CartMovement>();


    }

    // Update is called once per frame
    void Update()
    {
        if (_cart.Target)
        {
           
        }
    }
}
