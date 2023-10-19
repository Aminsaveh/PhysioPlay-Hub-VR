using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Grabber : MonoBehaviour {

    private GameObject selectedObject;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if(selectedObject == null) {
                RaycastHit hit = CastRay();

                if(hit.collider != null) {
                    if (!hit.collider.CompareTag("drag")) {
                        return;
                    }

                    selectedObject = hit.collider.gameObject;
                    Cursor.visible = false;
                }
            } else {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

                selectedObject = null;
                Cursor.visible = true;
            }
        }

        if(selectedObject != null) {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);


            Vector3 localUp = transform.up;
            Vector3 worldUp = Vector3.up;
            float dotProduct = Vector3.Dot(localUp, worldUp);

            if (Input.GetButtonDown("D")) {
                RotateCube(new Vector3(0,1f,0) * 90f);
            }
            if (Input.GetButtonDown("A"))
            {
                RotateCube(new Vector3(0,1f,0) * -90f);
            }
            if (Input.GetButtonDown("W")) {
                if(Mathf.Approximately(dotProduct,1.0f)||Mathf.Approximately(dotProduct,-1.0f))
                    RotateCube(new Vector3(1f,0,0) * 90f);
                else 
                    RotateCube(new Vector3(0f,0,1f) * 90f);
            }
            if (Input.GetButtonDown("S")) {
                RotateCube(new Vector3(1f,0,0) * -90f);
            }
        }
    }


    private void RotateCube(Vector3 rotationAxis)
    {
        selectedObject.transform.DORotate(selectedObject.transform.eulerAngles + rotationAxis, 1f, RotateMode.Fast);
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
}
