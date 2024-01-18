using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private float rotationSpeed = -500f; // Adjust speed as needed
    private Camera mainCamera;
    private bool isRotating = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Right mouse button press
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isRotating = true;
            }
        }

        // Right mouse button release
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        // Rotate the object if isRotating is true
        if (isRotating && Input.GetMouseButton(1))
        {
            float rotationY = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationY, 0, Space.World);
        }

        // Left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                OnObjectClicked();
            }
        }
    }

    private void OnObjectClicked()
    {
        // Implement your functionality here
        Debug.Log(gameObject.name + " was clicked");
    }
    
}
