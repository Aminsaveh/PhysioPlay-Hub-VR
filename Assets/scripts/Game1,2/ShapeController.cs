using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine;

public class ShapeController : MonoBehaviour
{

    private int rotationState = 0; // 0 represents the default front view
    private float rotationSpeed = 30; // Adjust the rotation speed as needed
    private bool isRotating = false;
    private bool isMoving = false;
    
    

    //[SerializeField] private List<Cube> cubes;

    [SerializeField] private Shape selectedShape;
    
    [SerializeField] private Level selectedLevel;

    [SerializeField] private int level = 1;

    public List<Level> levels;
    
    private OVRInput.Controller leftController;
    private OVRInput.Controller rightController;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    
    
    
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject crossGameObject;
    [SerializeField] private GameObject tickGameObject;
    



    private bool isTimerRunning;
    private float startTime;
    private int minutes;
    private int seconds;

    // Declare a variable for the laser pointer
    private LineRenderer lineRenderer;
    
    
    void SnapToPlaceholder()
    {
        if (selectedShape != null)
        {
            Shape shape = selectedShape;
            ShapePlaceholder bestFit = null;
            float maxOverlap = 0;
            foreach (var placeholder in selectedLevel.placeholders)
            {
                Collider cubeCollider = selectedShape.GetComponent<Collider>();
                Collider placeholderCollider = placeholder.collider;
                if (cubeCollider.bounds.Intersects(placeholderCollider.bounds))
                {
                    float overlapVolume = GetOverlapVolume(cubeCollider.bounds, placeholderCollider.bounds);
                    if (maxOverlap < overlapVolume)
                    {
                        maxOverlap = overlapVolume;
                        bestFit = placeholder;
                    }
                }
            }
            
            if (!Mathf.Approximately(maxOverlap, 0) && bestFit != null &&
                (bestFit.shape == null || bestFit.shape == shape))
            {
                isMoving = true;
                shape.transform.DOMove(bestFit.transform.position, 1f).OnComplete(() =>
                    {
                        isMoving = false;
                    bool isGameFinished = CheckIsGameFinished();
                    if (!isGameFinished)
                    {
                        CheckAreAllSetWrong();
                    }
                    }
                   );
                ShapePlaceholder pastPlaceholder = selectedLevel.placeholders.Find(x => x.shape == shape);
                if (pastPlaceholder != null)
                {
                    pastPlaceholder.shape = null;
                }

                bestFit.shape = shape;
                
            }
            else
            {
                ShapePlaceholder pastPlaceholder = selectedLevel.placeholders.Find(x => x.shape == shape);
                if (pastPlaceholder != null)
                {
                    pastPlaceholder.shape = null;
                }

                shape.transform.position = shape.initialPosition;
            }

            TogglePlaceholders();
        }
        
    }

    private void Start()
    {
        GameAnalytics.Initialize();
        TurnAllLevelsOff();
        SetSelectedLevel();
        TogglePlaceholderColliders(false);
        crossGameObject.gameObject.SetActive(false);
        tickGameObject.gameObject.SetActive(false);
        
        // Initialize the controller variables
        leftController = OVRInput.Controller.LTouch;
        rightController = OVRInput.Controller.RTouch;

        // Initialize the laser pointer variable
        lineRenderer = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (CanTakeAction())
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
                    if (!hit.collider.CompareTag("drag")) {
                        return;
                    }
                    selectedShape = hit.collider.gameObject.GetComponent<Shape>();
                    TogglePlaceholderColliders(true);
                }
            }
            // Left controller button release
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, leftController))
            {
                SetRotationRound();
                SnapToPlaceholder();
                selectedShape = null;
                TogglePlaceholderColliders(false);
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
                    selectedShape = hit.collider.gameObject.GetComponent<Shape>();
                    TogglePlaceholderColliders(true);
                }
            }
            // Right controller button release
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                SetRotationRound();
                SnapToPlaceholder();
                selectedShape = null;
                TogglePlaceholderColliders(false);
                // Hide the laser pointer
                HideLaser();
            }

            // Move the object with the left controller
            if (selectedShape != null & OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController))
            {
                RaycastHit hit;
                // Get the position and direction of the left controller game object
                Vector3 _position = left.transform.position;
                Vector3 direction = left.transform.forward;
                Ray ray = new Ray(_position, direction);
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, ray.origin + ray.direction * 4f);
                Vector3 position = lineRenderer.GetPosition(1);
                Vector3 worldPosition = transform.TransformPoint(position);
                var transform1 = selectedShape.transform;
                transform1.position = new Vector3(transform1.position.x, worldPosition.y, worldPosition.z);
                float rotX = OVRInput.Get(OVRInput.Axis2D.Any, leftController).x * 20 * Mathf.Deg2Rad;
                float rotY = OVRInput.Get(OVRInput.Axis2D.Any, leftController).y * 20 * Mathf.Deg2Rad;
                selectedShape.transform.RotateAround(Vector3.up, -rotX);
                selectedShape.transform.RotateAround(Vector3.right, rotY);
                
            } else if (selectedShape != null & OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                RaycastHit hit;
                // Get the position and direction of the left controller game object
                Vector3 _position = right.transform.position;
                Vector3 direction = right.transform.forward;
                Ray ray = new Ray(_position, direction);
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, ray.origin + ray.direction * 4f);
                Vector3 position = lineRenderer.GetPosition(1);
                Vector3 worldPosition = transform.TransformPoint(position);
                var transform1 = selectedShape.transform;
                transform1.position = new Vector3(transform1.position.x, worldPosition.y, worldPosition.z);
                float rotX = OVRInput.Get(OVRInput.Axis2D.Any, rightController).x * 20 * Mathf.Deg2Rad;
                float rotY = OVRInput.Get(OVRInput.Axis2D.Any, rightController).y * 20 * Mathf.Deg2Rad;
                selectedShape.transform.RotateAround(Vector3.up, -rotX);
                selectedShape.transform.RotateAround(Vector3.right, rotY);
               
            }
        }
        if (isTimerRunning)
        { 
            float timeElapsed = Time.time - startTime;
            string formattedTime = FormatTime(timeElapsed);
            timerText.text = formattedTime;
        }
    }

    bool CanTakeAction()
    {
        return !isMoving && !isRotating;
    }
    
    // Add a new method to show the laser pointer
    public void ShowLaser(Ray ray)
    {
        // Enable the line renderer
        lineRenderer.enabled = true;

        // Set the start and end positions of the line renderer
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 2f);
    }

    // Add a new method to hide the laser pointer
    public void HideLaser()
    {
        // Disable the line renderer
        lineRenderer.enabled = false;
    }
    
     void SetRotationRound()
    {
        if (selectedShape != null)
        {
            Shape cube = selectedShape;
            isRotating = true;
            Vector3 rotation = cube.transform.eulerAngles;
            cube.transform.DORotate(new Vector3(Mathf.Round(rotation.x/90.0f) * 90.0f,
                    Mathf.Round(rotation.y/90.0f) * 90.0f,
                    Mathf.Round(rotation.z/90.0f) * 90.0f),
                1f) .OnComplete(() =>
            {
                isRotating = false;
            });
        }
    }
     

    
    private float GetOverlapVolume(Bounds a, Bounds b)
    {
        Vector3 min;
        Vector3 max;
        float volume;

        // The min and max points
        var minA = a.min;
        var maxA = a.max;
        var minB = b.min;
        var maxB = b.max;
        

        min.x = Mathf.Max(minA.x, minB.x);
        min.y = Mathf.Max(minA.y, minB.y);
        min.z = Mathf.Max(minA.z, minB.z);

        max.x = Mathf.Min(maxA.x, maxB.x);
        max.y = Mathf.Min(maxA.y, maxB.y);
        max.z = Mathf.Min(maxA.z, maxB.z);
        
        // The diagonal of this overlap box itself
        var diagonal = max - min;
        volume = diagonal.x * diagonal.y * diagonal.z;
        return volume;
    }

    private void TogglePlaceholderColliders(bool isEnabled)
    {
        foreach (var placeholder in selectedLevel.placeholders)
        {
            placeholder.collider.enabled = isEnabled;
        }
    }
    
    private void TogglePlaceholders()
    {
        foreach (var placeholder in selectedLevel.placeholders)
        {
            if (placeholder.shape != null)
            {
                placeholder.meshRenderer.enabled = false;
            }
            else
            {
                placeholder.meshRenderer.enabled = true; 
            }
        }
    }



    private bool CheckIsGameFinished()
    {
        bool areAllShapesSet = true;
        foreach (ShapePlaceholder placeholder in selectedLevel.placeholders)
        {
            if (placeholder.shape == null)
            {
                Debug.Log("here");
                areAllShapesSet = false;
                break;
            }

            if (placeholder.shape.form != placeholder.form)
            {
                Debug.Log("here1");
                areAllShapesSet = false;
                break;
            }

            areAllShapesSet = IsRotationCorrect(placeholder.shape);
            
            
        }

        if (areAllShapesSet && selectedLevel.placeholders.Count > 0)
        {
            Debug.Log("WIN");
            SetNextLevel();
        }

        return areAllShapesSet;
    }
    
    
    private void CheckAreAllSetWrong()
    {
        bool areAllShapesSetWrong = true;
        foreach (ShapePlaceholder placeholder in selectedLevel.placeholders)
        {
            if (placeholder.shape == null)
            {
                areAllShapesSetWrong = false;
                break;
            }
        }
        if (areAllShapesSetWrong && selectedLevel.placeholders.Count > 0)
        {
           crossGameObject.SetActive(true);
        }
        else
        {
            crossGameObject.SetActive(false);   
        }
    }


    private bool IsRotationCorrect(Shape shape)
    {
        if (shape.form == Form.Rectangle)
        {
            if (!AreTwoSame(shape.transform.rotation.eulerAngles.x % 180, 0.0f) ||
                !AreTwoSame(shape.transform.rotation.eulerAngles.y % 180, 0.0f))
            {
                return false;
            }
        }
            
        if (shape.form == Form.Pyramid)
        {
            if (!AreTwoSame(shape.transform.rotation.eulerAngles.x % 360 , 0.0f) ||
                !AreTwoSame(shape.transform.rotation.eulerAngles.z % 360 , 0.0f))
            {
                return false;
            }
        }

        return true;
    }


    private void SetSelectedLevel()
    {
        StartStopwatch();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Game-2_Level_" + level);
        selectedLevel = levels[level-1];
        selectedLevel.gameObject.SetActive(true);
    }


    private bool AreTwoSame(float val1, float val2)
    {
        return Mathf.Abs(val1 - val2) < 1f;
    }
    

    private async void SetNextLevel()
    {
        StopStopwatch();
        tickGameObject.gameObject.SetActive(true);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Game-2_Level_" + level);
        await Task.Delay(3000);
        ResetShapes();
        selectedLevel.gameObject.SetActive(false);
        level++;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Game-2_Level_" + level);
        selectedLevel = levels[level - 1];
        selectedLevel.gameObject.SetActive(true);
        StartStopwatch();
        tickGameObject.gameObject.SetActive(false);

    }


    private void ResetShapes()
    {
        foreach (Shape shape in selectedLevel.shapes)
        {
            shape.transform.position = shape.initialPosition;
        }
    }

    private void TurnAllLevelsOff()
    {
        foreach (Level level in levels)
        {
            level.gameObject.SetActive(false);
        }
    }
    
    
    string FormatTime(float timeToFormat)
    {
        minutes = (int)timeToFormat / 60;
        seconds = (int)timeToFormat % 60;
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        return formattedTime;
    }
    
    
    public void StartStopwatch()
    {
        isTimerRunning = true;
        startTime = Time.time;
        minutes = 0;
        seconds = 0;
    }

    public void StopStopwatch()
    {
        isTimerRunning = false;
    }
}
