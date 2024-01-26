using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OculusSampleFramework;
using UnityEngine;
using UnityEngine.XR;

public class ObjectController : MonoBehaviour
{
    private float rotationSpeed = -500f; // Adjust speed as needed
    private Camera mainCamera;
    private GameObject selectedObject;
    private bool isRotating = false;
    private int number = Int32.MaxValue - 1;
    private string tag = "Hello";
    [SerializeField] private GameManager _gameManager;

    // Declare variables for the left and right controllers
    private OVRInput.Controller leftController;
    private OVRInput.Controller rightController;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;

    // Declare a variable for the laser pointer
    private LineRenderer lineRenderer;

    public int Number
    {
        get => number;
        set => number = value;
    }

    public string Tag
    {
        get => tag;
        set => tag = value;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        // Initialize the controller variables
        leftController = OVRInput.Controller.LTouch;
        rightController = OVRInput.Controller.RTouch;

        // Initialize the laser pointer variable
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    public void ShowLaser(Ray ray)
    {
        // Enable the line renderer
        lineRenderer.enabled = true;

        // Set the start and end positions of the line renderer
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 2f);
    }

    // Define a function to hide the laser pointer
    public void HideLaser()
    {
        // Disable the line renderer
        lineRenderer.enabled = false;
    }

    private void Update()
    {
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
                if (!hit.transform.CompareTag("Untagged")) // Check if the object's tag is not "Untagged"
                {
                    OnObjectClicked(hit.transform.gameObject);
                }
            }
        }
        
        // Left controller button release
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, leftController))
        {
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
                if (!hit.transform.CompareTag("Untagged")) // Check if the object's tag is not "Untagged"
                {
                    
                    selectedObject = hit.transform.gameObject;
                    isRotating = true;
                }
            }
        }

        // Right controller button release
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, rightController))
        {
            // Hide the laser pointer
            HideLaser();

            isRotating = false;
            selectedObject = null;
        }

        // Rotate the object if isRotating is true
        if (isRotating && selectedObject != null)
        {
            Debug.Log("Hi");
            float rotationY = OVRInput.Get(OVRInput.Axis2D.Any, rightController).x * rotationSpeed * Time.deltaTime;
            selectedObject.transform.Rotate(0, rotationY, 0, Space.World);
        }
    }

    private async void OnObjectClicked(GameObject obj)
    {
        await Task.Delay(500);
        HideLaser();
        int.TryParse(obj.tag, out number); // Try parsing the tag to an int, if possible
        tag = obj.tag;

        _gameManager.CheckSelected();
        
    }
    
}
