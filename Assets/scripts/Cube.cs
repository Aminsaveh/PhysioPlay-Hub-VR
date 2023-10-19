using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

public enum CubeFace
{
    Front,
    Back,
    Top,
    Bottom,
    Left,
    Right
}

public class Cube : MonoBehaviour
{
    public CubeFace cubeFace = CubeFace.Front;
    public Vector3 initialPosition;
    public Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
        initialPosition = transform.position;
    }
}


