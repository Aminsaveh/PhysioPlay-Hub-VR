
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
    
    [SerializeField] private List<Placeholder> placeholders;

    //[SerializeField] private List<Cube> cubes;

    [SerializeField] private Cube selectedCube;

    [SerializeField] private int level = 1;
    
    [SerializeField] private PuzzleSize puzzleSize; 
    
    private Vector3 frontRotation = new Vector3(0,0,0);
    private Vector3 topRotation = new Vector3(-90,0,90);
    private Vector3 backRotation = new Vector3(0,-90,0);
    private Vector3 bottomRotation = new Vector3(-90,0,0);
    private Vector3 rightRotation = new Vector3(0,90,0);
    private Vector3 leftRotation = new Vector3(0,-90,-180);




    void SnapToPlaceholder()
    {
        Placeholder bestFit = null;
        float maxOverlap = 0;
        foreach (var placeholder in placeholders)
        {
            Collider cubeCollider = selectedCube.GetComponent<Collider>();
            Collider placeholderCollider = placeholder.GetComponent<Collider>();
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
        //float distanceToPlaceholder = Vector3.Distance(selectedCube.transform.position, placeholder.transform.position);
        if (!Mathf.Approximately(maxOverlap,0) && bestFit != null && (bestFit.cube == null || bestFit.cube == selectedCube))
        {
            isMoving = true;
            selectedCube.transform.DOMove(bestFit.transform.position,2f) .OnComplete(() => isMoving = false);
            Placeholder pastPlaceholder = placeholders.Find(x => x.cube == selectedCube);
            if (pastPlaceholder != null)
            {
                pastPlaceholder.cube = null;
            }
            bestFit.cube = selectedCube;
        }
        else
        {
            Placeholder pastPlaceholder = placeholders.Find(x => x.cube == selectedCube);
            if (pastPlaceholder != null)
            {
                pastPlaceholder.cube = null;
            }
            selectedCube.transform.position = selectedCube.initialPosition;
        }

        Debug.Log("Here");
        CheckIsGameFinished();
    }

    private void Start()
    {
        TogglePlaceholderColliders(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanTakeAction())
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                RaycastHit hit = CastRay();

                if(hit.collider != null) {
                    if (!hit.collider.CompareTag("drag")) {
                        return;
                    }
                    selectedCube = hit.collider.gameObject.GetComponent<Cube>();
                    TogglePlaceholderColliders(true);
                    Cursor.visible = false;
                }
            }
            if (Input.GetMouseButtonUp(0))
                {
                    SnapToPlaceholder();
                    Cursor.visible = true;
                    selectedCube = null;
                    TogglePlaceholderColliders(false);
                }
            
                if (Input.GetMouseButtonUp(1))
                {
                    SetRotationRound();
                    SnapToPlaceholder();
                    Cursor.visible = true;
                }

                if (Input.GetMouseButton(0))
                {
                    
                    if(selectedCube != null){
                        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                            Camera.main.WorldToScreenPoint(selectedCube.transform.position).z);
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                        selectedCube.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
                    }
                }

                if (Input.GetMouseButton(1))
                {
                    if (selectedCube != null)
                    {
                        float rotX = Input.GetAxis("Mouse X") * 20 * Mathf.Deg2Rad;
                        float rotY = Input.GetAxis("Mouse Y") * 20 * Mathf.Deg2Rad;
                        selectedCube.transform.RotateAround(Vector3.up, -rotX);
                        selectedCube.transform.RotateAround(Vector3.right, rotY);
                        CalculateFacedEdge();
                    }
                }
        }
    }
    
    bool CanTakeAction()
    {
        return !isMoving && !isRotating;
    }
    
    private RaycastHit CastRay() {
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
    
     void SetRotationRound()
    {
        isRotating = true;
        Vector3 rotation = selectedCube.transform.eulerAngles;
        selectedCube.transform.DORotate(new Vector3(Mathf.Round(rotation.x/90.0f) * 90.0f,
            Mathf.Round(rotation.y/90.0f) * 90.0f,
            Mathf.Round(rotation.z/90.0f) * 90.0f),
            2f) .OnComplete(() => isRotating = false);
        // When the rotation is complete, set isRotating to false;
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
        foreach (var placeholder in placeholders)
        {
            placeholder.GetComponent<Collider>().enabled = isEnabled;
        }
    }



    private void CheckIsGameFinished()
    {
        if(placeholders.Find(it => it.cube == null) != null) return;
        foreach (var placeholder in placeholders)
        {
            if (level == 1)
            {
                Debug.Log("ImageIndex = 1");
                if(placeholder.cube.cubeFace != CubeFace.Front) return;
                Debug.Log(placeholder.cube.transform.rotation.eulerAngles.Equals(frontRotation));
                if(!placeholder.cube.transform.rotation.eulerAngles.Equals(frontRotation)) return;
                Debug.Log(placeholder.cube.index + " | " + placeholder.index);
                if(placeholder.cube.index != placeholder.index) return;
            
            }

            else if (level == 2)
            {
                Debug.Log("ImageIndex = 2");
                if(placeholder.cube.cubeFace != CubeFace.Top) return;
                if(placeholder.cube.transform.rotation.eulerAngles.Equals(topRotation)) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            
            else if (level == 3)
            {
                Debug.Log("ImageIndex = 3");
                if(placeholder.cube.cubeFace != CubeFace.Back) return;
                if(placeholder.cube.transform.rotation.eulerAngles.Equals(backRotation)) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else if (level == 4)
            {
                Debug.Log("ImageIndex = 4");
                if(placeholder.cube.cubeFace != CubeFace.Right) return;
                if(placeholder.cube.transform.rotation.eulerAngles.Equals(rightRotation)) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else if (level == 5)
            {
                Debug.Log("ImageIndex = 5");
                if(placeholder.cube.cubeFace != CubeFace.Left) return;
                if(placeholder.cube.transform.rotation.eulerAngles.Equals(leftRotation)) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else if (level == 6)
            {
                Debug.Log("ImageIndex = 6");
                if(placeholder.cube.cubeFace != CubeFace.Bottom) return;
                if(placeholder.cube.transform.rotation.eulerAngles.Equals(bottomRotation)) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else
            {
                Debug.Log("else");
                return;
            }
        }

        level++;
        Debug.Log("Win!");
    }
    
}
