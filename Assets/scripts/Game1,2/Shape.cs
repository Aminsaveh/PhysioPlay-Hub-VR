using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum Form
{
    Square,
    
    Rectangle,
    Pyramid
}

public class Shape : MonoBehaviour
{
    public Form form;
    public Collider collider;
    public Quaternion initialRotation;
    public Vector3 initialPosition;


    private void Start()
    {
        initialPosition = this.transform.position;
    }
}

