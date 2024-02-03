
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using TMPro;

public class CubeController : MonoBehaviour
{
    
    private enum PuzzleSize
    {
        Small = 4,
        Medium = 9,
        Large = 16
    }
    
    private int rotationState = 0; // 0 represents the default front view
    private float rotationSpeed = 30; // Adjust the rotation speed as needed
    private bool isRotating = false;
    private bool isMoving = false;

    public Map map2x2;
    public Map map3x3;
    public Map map4x4;
        
    public Cube cubePrefab;


    public Material[] images;
    
    //[SerializeField] private List<Cube> cubes;

    [SerializeField] private Cube selectedCube;
    
    [SerializeField] private Map selectedMap;

    [SerializeField] private int level = 0;
    
    [SerializeField] private PuzzleSize puzzleSize; 
    
    [SerializeField] private MeshRenderer puzzleOriginalImage;

    private Quaternion frontRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion topRotation = Quaternion.Euler(0,0,90);
    private Quaternion backRotation = Quaternion.Euler(0,-180,0);
    private Quaternion bottomRotation =Quaternion.Euler(0,0,-90);
    private Quaternion rightRotation = Quaternion.Euler(0,90,0);
    private Quaternion leftRotation = Quaternion.Euler(0,-90,0);
    
    
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
        if (selectedCube != null)
        {
            Cube cube = selectedCube;
            Placeholder bestFit = null;
            float maxOverlap = 0;
            foreach (var placeholder in selectedMap.placeholders)
            {
                Collider cubeCollider = cube.collider;
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
                (bestFit.cube == null || bestFit.cube == cube))
            {
                isMoving = true;
                cube.transform.DOMove(bestFit.transform.position, 1f).OnComplete(() =>
                    {
                        isMoving = false;
                    bool isGameFinished = CheckIsGameFinished();
                    if (!isGameFinished)
                    {
                        CheckAreAllSetWrong();
                    }
                }
                   );
                Placeholder pastPlaceholder = selectedMap.placeholders.Find(x => x.cube == cube);
                if (pastPlaceholder != null)
                {
                    pastPlaceholder.cube = null;
                }

                bestFit.cube = cube;
            }
            else
            {
                Placeholder pastPlaceholder = selectedMap.placeholders.Find(x => x.cube == cube);
                if (pastPlaceholder != null)
                {
                    pastPlaceholder.cube = null;
                }

                cube.transform.position = cube.initialPosition;
            }
        }
        
    }

    private void Start()
    {
        GameAnalytics.Initialize();

        TurnAllMapsOff();
        SetSelectedMap();
        TogglePlaceholderColliders(false);
        GenerateCubes((int)puzzleSize); 
        cubePrefab.gameObject.SetActive(false);
        SetNextLevel();
        crossGameObject.gameObject.SetActive(false);
        tickGameObject.gameObject.SetActive(false);
        
        // Initialize the controller variables
        leftController = OVRInput.Controller.LTouch;
        rightController = OVRInput.Controller.RTouch;

        // Initialize the laser pointer variable
        lineRenderer = GetComponent<LineRenderer>();

        

    }

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
                    selectedCube = hit.collider.gameObject.GetComponent<Cube>();
                    TogglePlaceholderColliders(true);
                }
            }
            // Left controller button release
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, leftController))
            {
                SetRotationRound();
                SnapToPlaceholder();
                selectedCube = null;
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
                    selectedCube = hit.collider.gameObject.GetComponent<Cube>();
                    TogglePlaceholderColliders(true);
                }
            }
            // Right controller button release
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, rightController))
            {
                SetRotationRound();
                SnapToPlaceholder();
                selectedCube = null;
                TogglePlaceholderColliders(false);
                // Hide the laser pointer
                HideLaser();
            }

            // Move the object with the left controller
            if (selectedCube != null & OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, leftController))
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
                var transform1 = selectedCube.transform;
                transform1.position = new Vector3(transform1.position.x, worldPosition.y, worldPosition.z);
                float rotX = OVRInput.Get(OVRInput.Axis2D.Any, leftController).x * 20 * Mathf.Deg2Rad;
                float rotY = OVRInput.Get(OVRInput.Axis2D.Any, leftController).y * 20 * Mathf.Deg2Rad;
                selectedCube.transform.RotateAround(Vector3.up, -rotX);
                selectedCube.transform.RotateAround(Vector3.right, rotY);
                
            } else if (selectedCube != null & OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, rightController))
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
                var transform1 = selectedCube.transform;
                transform1.position = new Vector3(transform1.position.x, worldPosition.y, worldPosition.z);
                float rotX = OVRInput.Get(OVRInput.Axis2D.Any, rightController).x * 20 * Mathf.Deg2Rad;
                float rotY = OVRInput.Get(OVRInput.Axis2D.Any, rightController).y * 20 * Mathf.Deg2Rad;
                selectedCube.transform.RotateAround(Vector3.up, -rotX);
                selectedCube.transform.RotateAround(Vector3.right, rotY);
               
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
        lineRenderer.SetPosition(1, ray.origin + ray.direction);
    }

    // Add a new method to hide the laser pointer
    public void HideLaser()
    {
        // Disable the line renderer
        lineRenderer.enabled = false;
    }
    
     public void SetRotationRound()
    {
        if (selectedCube != null)
        {
            Cube cube = selectedCube;
            isRotating = true;
            Vector3 rotation = cube.transform.eulerAngles;
            cube.transform.DORotate(new Vector3(Mathf.Round(rotation.x/90.0f) * 90.0f,
                    Mathf.Round(rotation.y/90.0f) * 90.0f,
                    Mathf.Round(rotation.z/90.0f) * 90.0f),
                1f) .OnComplete(() =>
            {
                isRotating = false;
                CalculateFacedEdge();
            });
        }
    }

    void CalculateFacedEdge()
    {
        // Get the camera's forward vector (assuming the camera is the main camera)
        Vector3 cameraForward = Camera.main.transform.forward;

        // Calculate the dot product between the camera's forward vector and each cube face normal
        float frontDot = Vector3.Dot(cameraForward, selectedCube.transform.forward);
        float backDot = Vector3.Dot(cameraForward, -selectedCube.transform.forward);
        float topDot = Vector3.Dot(cameraForward, selectedCube.transform.up);
        float bottomDot = Vector3.Dot(cameraForward, -selectedCube.transform.up);
        float leftDot = Vector3.Dot(cameraForward, -selectedCube.transform.right);
        float rightDot = Vector3.Dot(cameraForward, selectedCube.transform.right);
        
        // Find the maximum dot product, which corresponds to the closest cube face
        float maxDot = Mathf.Max(frontDot, backDot, topDot, bottomDot, leftDot, rightDot);

        // Determine the faced edge based on the maximum dot product
        if (Mathf.Approximately(maxDot , frontDot))
        {
            selectedCube.cubeFace = CubeFace.Right;
        }
        else if (Mathf.Approximately(maxDot , backDot))
        {
            selectedCube.cubeFace = CubeFace.Left;
        }
        else if (Mathf.Approximately(maxDot , topDot))
        {
            selectedCube.cubeFace = CubeFace.Bottom;
        }
        else if (Mathf.Approximately(maxDot , bottomDot))
        {
            selectedCube.cubeFace = CubeFace.Top;
        }
        else if (Mathf.Approximately(maxDot , leftDot))
        {
            selectedCube.cubeFace = CubeFace.Back;
        }
        else
        {
            selectedCube.cubeFace = CubeFace.Front;
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
        foreach (var placeholder in selectedMap.placeholders)
        {
            placeholder.collider.enabled = isEnabled;
        }
    }



    private bool CheckIsGameFinished()
    {
        if(selectedMap.placeholders.Find(it => it.cube == null) != null) return false;
        foreach (var placeholder in selectedMap.placeholders)
        {
            if (level == 1)
            {
                if(placeholder.cube.cubeFace != CubeFace.Front) return false;
             
                if(Quaternion.Angle(placeholder.cube.transform.rotation,frontRotation)>=1f) return false;
          
                if(placeholder.cube.index != placeholder.index) return false;
            
            }

            else if (level == 2)
            {
                if(placeholder.cube.cubeFace != CubeFace.Top) return false;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,topRotation)>=1f) return false;
                if(placeholder.cube.index != placeholder.index) return false;
            }
            
            else if (level == 3)
            {
                Debug.Log("ImageIndex = 3");
                if(placeholder.cube.cubeFace != CubeFace.Back) return false;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,backRotation)>=1f) return false;
                if(placeholder.cube.index != placeholder.index) return false;
            }
            else if (level == 4)
            {
                Debug.Log("ImageIndex = 4");
                if(placeholder.cube.cubeFace != CubeFace.Right) return false;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,rightRotation)>=1f) return false;
                if(placeholder.cube.index != placeholder.index) return false;
            }
            else if (level == 5)
            {
                Debug.Log("ImageIndex = 5");
                if(placeholder.cube.cubeFace != CubeFace.Left) return false;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,leftRotation)>=1f) return false;
                if(placeholder.cube.index != placeholder.index) return false;
            }
            else if (level == 6)
            {
                Debug.Log("ImageIndex = 6");
                if(placeholder.cube.cubeFace != CubeFace.Bottom) return false;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,bottomRotation)>=1f) return false;
                if(placeholder.cube.index != placeholder.index) return false;
            }
            else
            {
                Debug.Log("else");
                return false;
            }
            Debug.Log("-----");
        }
        
        SetNextLevel();
        Debug.Log("Win!");
        return true;
    }


    private void CheckAreAllSetWrong()
    {
        bool areAllSetWrong = true;
        foreach (var placeholder in selectedMap.placeholders)
        {
            if (placeholder.cube == null)
            {
                areAllSetWrong = false;
            }
        }

        if (areAllSetWrong)
        {
            crossGameObject.gameObject.SetActive(true);
        }
        else
        {
            crossGameObject.gameObject.SetActive(false);
        }
        
    }


    private void SetSelectedMap()
    {
        
        switch (puzzleSize)
        {
            case PuzzleSize.Small : selectedMap = map2x2; break;
            case PuzzleSize.Medium : selectedMap = map3x3; break;
            case PuzzleSize.Large : selectedMap = map4x4; break;
        }
        Debug.Log(puzzleSize);
        selectedMap.gameObject.SetActive(true);
    }
    
    private void GenerateCubes(int level)
    {
        int sideLength = Mathf.RoundToInt(Mathf.Sqrt(level));  // For 4 it's 2x2, for 9 it's 3x3, for 16 it's 4x4
        int cubeNumber = 1;

        for (int z = 0; z < sideLength; z++)
        {
            for (int x = 0; x < sideLength; x++)
            {
                Cube cube = Instantiate(cubePrefab, selectedMap.cubePlaceholders[cubeNumber-1].transform.position, Quaternion.identity);

                cube.index = cubeNumber;
                cube.name = cubeNumber.ToString();  // Naming the cube
                cubeNumber++;

                AdjustUVs(cube.gameObject, x, z, sideLength);
            }
        }
    }

    private void AdjustUVs(GameObject cube, int x, int z, int sideLength)
    {
        foreach (MeshFilter meshFilter in cube.GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = meshFilter.mesh;
            Vector2[] uvs = mesh.uv;
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i].x = (uvs[i].x / sideLength) + (float)x / sideLength;
                uvs[i].y = (uvs[i].y / sideLength) + (float)z / sideLength;
            }

            mesh.uv = uvs;
        }
    }


    private async void SetNextLevel()
    {   
        StopStopwatch();
        tickGameObject.SetActive(true);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Game-1_Level_" + level);
        await Task.Delay(3000);
        StartStopwatch();
        level++;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Game-1_Level_" + level);
        puzzleOriginalImage.material = images[level-1];
        for (int i = 0; i < selectedMap.placeholders.Count; i++)
        {
            if (selectedMap.placeholders[i].cube != null)
            {
                selectedMap.placeholders[i].cube.transform
                    .DOMove(selectedMap.cubePlaceholders[i].transform.position, 1f);
                selectedMap.placeholders[i].cube.initialPosition = selectedMap.cubePlaceholders[i].transform.position;
                selectedMap.placeholders[i].cube.initialRotation = selectedMap.cubePlaceholders[i].transform.rotation;
                selectedMap.placeholders[i].cube = null;
            }

        }
        tickGameObject.SetActive(false);
    }

    private void TurnAllMapsOff()
    {
        map2x2.gameObject.SetActive(false);
        map3x3.gameObject.SetActive(false);
        map4x4.gameObject.SetActive(false);
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
