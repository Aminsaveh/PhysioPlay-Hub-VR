using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LevelIntro : MonoBehaviour
{
    private float rotationSpeed = 1f; // Degrees per frame
    private float targetAngle = -90f; // Rotation target

    [SerializeField] private GameObject[] doors;

    private float AngleReturner(GameObject gameObject)
    {
        Quaternion localRotation = gameObject.transform.localRotation;
        Vector3 localEulerAngles = localRotation.eulerAngles;

        return localEulerAngles.y;
    }

    void Update()
    {
        
        // Gradually rotate towards target angle
        if (Math.Abs(AngleReturner(doors[0]) - 90) < 180)
        {
            doors[0].transform.Rotate(Vector3.up * rotationSpeed);
            doors[1].transform.Rotate(Vector3.down * rotationSpeed);
        }else
        {
            doors[0].transform.rotation = Quaternion.Euler(0f, targetAngle, 0f); // Snap to target angle if close enough
            doors[1].transform.rotation = Quaternion.Euler(0f, -targetAngle, 0f);
        }
        
    }
}
