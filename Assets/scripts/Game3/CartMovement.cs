using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartMovement : MonoBehaviour
{
    
    // The array of checkpoints to move and rotate towards
    [SerializeField] private Transform[] checkpoints;

    private bool _startGame;

    public bool Target => _startGame;

    // The speed of movement and rotation
    private float moveSpeed = 0.2f;
    private float rotateSpeed = 0.5f;

    // The current target checkpoint index
    private int target;

    // The current lerp ratio
    private float t;

    // The initial position and rotation of the object
    private Vector3 startPos;
    private Quaternion startRot;

    void Start () {
        // Set the initial target to the first checkpoint index
        target = 0;
        // Set the initial lerp ratio to zero
        t = 0f;
        // Set the initial position and rotation to the current transform
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update () {
        // If the target index is within the array bounds
        if (target < checkpoints.Length) {
            // Move and rotate the object towards the target checkpoint using the initial position and rotation
            transform.position = Vector3.Lerp (startPos, checkpoints[target].position, t);
            transform.rotation = Quaternion.Lerp (startRot, checkpoints[target].rotation, t);
            // Increase the lerp ratio by the move and rotate speed times the fixed delta time
            t += (moveSpeed + rotateSpeed) * Time.fixedDeltaTime;
            // Clamp the lerp ratio to the range [0, 1]
            t = Mathf.Clamp01(t);
            // If the lerp ratio reaches or exceeds one
            if (t >= 1f) {
                // Increment the target index
                target++;
                // Reset the lerp ratio to zero
                t = 0f;
                // Update the initial position and rotation to the current transform
                startPos = transform.position;
                startRot = transform.rotation;
            }
        }
        else
        {
            _startGame = true;
        }
        
    }
    
}
