using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
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
                    CheckIsGameFinished();
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
        TurnAllLevelsOff();
        SetSelectedLevel();
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
                    selectedShape = hit.collider.gameObject.GetComponent<Shape>();
                    TogglePlaceholderColliders(true);
                    Cursor.visible = false;
                }
            }
            if (Input.GetMouseButtonUp(0))
                {
                    SnapToPlaceholder();
                    Cursor.visible = true;
                    selectedShape = null;
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
                    if(selectedShape != null){
                        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                            Camera.main.WorldToScreenPoint(selectedShape.transform.position).z);
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                        selectedShape.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
                    }
                }

                if (Input.GetMouseButton(1))
                {
                    if (selectedShape != null)
                    {
                        float rotX = Input.GetAxis("Mouse X") * 20 * Mathf.Deg2Rad;
                        float rotY = Input.GetAxis("Mouse Y") * 20 * Mathf.Deg2Rad;
                        selectedShape.transform.RotateAround(Vector3.up, -rotX);
                        selectedShape.transform.RotateAround(Vector3.right, rotY);
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



    private void CheckIsGameFinished()
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
        selectedLevel = levels[level-1];
        selectedLevel.gameObject.SetActive(true);
    }


    private bool AreTwoSame(float val1, float val2)
    {
        return Mathf.Abs(val1 - val2) < 1f;
    }
    

    private async void SetNextLevel()
    {
        await Task.Delay(3000);
        ResetShapes();
        selectedLevel.gameObject.SetActive(false);
        level++;
        selectedLevel = levels[level - 1];
        selectedLevel.gameObject.SetActive(true);

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
}
