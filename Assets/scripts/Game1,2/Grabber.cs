using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Grabber : MonoBehaviour {

    private GameObject selectedObject;
    
    private OVRInput.Controller leftController;
    private OVRInput.Controller rightController;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private CubeController _controller;

    // Declare a variable for the laser pointer
    private LineRenderer lineRenderer;

    private void Start()
    {
        // Initialize the controller variables
        leftController = OVRInput.Controller.LTouch;
        rightController = OVRInput.Controller.RTouch;

        // Initialize the laser pointer variable
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        
            // Left controller button press
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, leftController))
            {
                RaycastHit hit;
                // Get the position and direction of the left controller game object
                Vector3 position = left.transform.position;
                Vector3 direction = left.transform.forward;
                Ray ray = new Ray(position, direction);

                // Show the laser pointer
                ShowLaser(ray);

                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.CompareTag("drag")) {
                        return;
                    }

                    selectedObject = hit.collider.gameObject;
                }
            }
            // Left controller button release
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, leftController))
            {
                Vector3 position = lineRenderer.GetPosition(1);
                Vector3 worldPosition = transform.TransformPoint(position);
                selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

                selectedObject = null;
                // Hide the laser pointer
                HideLaser();
            }
            
            // Right controller button press
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                RaycastHit hit;
                // Get the position and direction of the right controller game object
                Vector3 position = right.transform.position;
                Vector3 direction = right.transform.forward;
                Ray ray = new Ray(position, direction);

                // Show the laser pointer
                ShowLaser(ray);

                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.CompareTag("drag")) {
                        return;
                    }

                    selectedObject = hit.collider.gameObject;
                    _controller.SetRotationRound();
                }
            }
            // Right controller button release
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                Vector3 position = lineRenderer.GetPosition(1);
                Vector3 worldPosition = transform.TransformPoint(position);
                selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

                selectedObject = null;
                // Hide the laser pointer
                HideLaser();
            }

            if(selectedObject != null) {
                Vector3 localUp = transform.up;
                Vector3 worldUp = Vector3.up;
                float dotProduct = Vector3.Dot(localUp, worldUp);

                // Rotate the object with the right controller thumbstick
                float rotX = OVRInput.Get(OVRInput.Axis2D.Any, rightController).x * 20 * Mathf.Deg2Rad;
                float rotY = OVRInput.Get(OVRInput.Axis2D.Any, rightController).y * 20 * Mathf.Deg2Rad;
                RotateCube(new Vector3(rotY, -rotX, 0) * 90f);
            }
        }
    
    

   

    private void RotateCube(Vector3 rotationAxis)
    {
        selectedObject.transform.DORotate(selectedObject.transform.eulerAngles + rotationAxis, 1f, RotateMode.Fast);
    }

    // Add a new method to show the laser pointer
    public void ShowLaser(Ray ray)
    {
        // Enable the line renderer
        lineRenderer.enabled = true;

        // Set the start and end positions of the line renderer
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 3.8f);
    }

    // Add a new method to hide the laser pointer
    public void HideLaser()
    {
        // Disable the line renderer
        lineRenderer.enabled = false;
    }
}
