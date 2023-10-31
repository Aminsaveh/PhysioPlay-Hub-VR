
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
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
                cube.transform.DOMove(bestFit.transform.position, 2f).OnComplete(() =>
                    {
                        isMoving = false;
                    CheckIsGameFinished();
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
        SetSelectedMap();
        TogglePlaceholderColliders(false);
        GenerateCubes((int)puzzleSize); 
        cubePrefab.gameObject.SetActive(false);
        SetNextLevel();

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
        if (selectedCube != null)
        {
            Cube cube = selectedCube;
            isRotating = true;
            Vector3 rotation = cube.transform.eulerAngles;
            cube.transform.DORotate(new Vector3(Mathf.Round(rotation.x/90.0f) * 90.0f,
                    Mathf.Round(rotation.y/90.0f) * 90.0f,
                    Mathf.Round(rotation.z/90.0f) * 90.0f),
                2f) .OnComplete(() =>
            {
                isRotating = false;
                CalculateFacedEdge();
                CheckIsGameFinished();
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



    private void CheckIsGameFinished()
    {
        if(selectedMap.placeholders.Find(it => it.cube == null) != null) return;
        Debug.Log("-----");
        foreach (var placeholder in selectedMap.placeholders)
        {
            Debug.Log("|||||");
            Debug.Log(placeholder.cube.cubeFace );
            Debug.Log(placeholder.cube.transform.rotation.eulerAngles + " | " + topRotation);
            Debug.Log(placeholder.cube.index + " | " + placeholder.index);
            Debug.Log("|||||");
            if (level == 1)
            {
                if(placeholder.cube.cubeFace != CubeFace.Front) return;
             
                if(Quaternion.Angle(placeholder.cube.transform.rotation,frontRotation)>=1f) return;
          
                if(placeholder.cube.index != placeholder.index) return;
            
            }

            else if (level == 2)
            {
                if(placeholder.cube.cubeFace != CubeFace.Top) return;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,topRotation)>=1f) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            
            else if (level == 3)
            {
                Debug.Log("ImageIndex = 3");
                if(placeholder.cube.cubeFace != CubeFace.Back) return;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,backRotation)>=1f) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else if (level == 4)
            {
                Debug.Log("ImageIndex = 4");
                if(placeholder.cube.cubeFace != CubeFace.Right) return;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,rightRotation)>=1f) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else if (level == 5)
            {
                Debug.Log("ImageIndex = 5");
                if(placeholder.cube.cubeFace != CubeFace.Left) return;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,leftRotation)>=1f) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else if (level == 6)
            {
                Debug.Log("ImageIndex = 6");
                if(placeholder.cube.cubeFace != CubeFace.Bottom) return;
                if(Quaternion.Angle(placeholder.cube.transform.rotation,bottomRotation)>=1f) return;
                if(placeholder.cube.index != placeholder.index) return;
            }
            else
            {
                Debug.Log("else");
                return;
            }
            Debug.Log("-----");
        }

        SetNextLevel();
        Debug.Log("Win!");
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


    private  async void SetNextLevel()
    {
        await Task.Delay(3000);
        level++;
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
        
    }


 
}
