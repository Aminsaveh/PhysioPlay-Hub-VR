using System;
using UnityEngine;

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

            if (Input.GetButton("D")) {
                selectedObject.transform.rotation = Quaternion.Lerp(selectedObject.transform.rotation, Quaternion.Euler(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 90f,
                    selectedObject.transform.rotation.eulerAngles.z), Time.deltaTime * 1f);
            }
            if (Input.GetButton("A")) {
                selectedObject.transform.rotation = Quaternion.Lerp(selectedObject.transform.rotation, Quaternion.Euler(
                        selectedObject.transform.rotation.eulerAngles.x,
                        selectedObject.transform.rotation.eulerAngles.y - 90f,
                        selectedObject.transform.rotation.eulerAngles.z), Time.deltaTime * 1f);
            }
            if (Input.GetButton("W")) {
                selectedObject.transform.rotation = Quaternion.Lerp(selectedObject.transform.rotation, Quaternion.Euler(
                        selectedObject.transform.rotation.eulerAngles.x + 90f,
                        selectedObject.transform.rotation.eulerAngles.y,
                        selectedObject.transform.rotation.eulerAngles.z), Time.deltaTime * 1f);
            }
            if (Input.GetButton("S")) {
                selectedObject.transform.rotation = Quaternion.Lerp(selectedObject.transform.rotation, Quaternion.Euler(
                        selectedObject.transform.rotation.eulerAngles.x -90f,
                        selectedObject.transform.rotation.eulerAngles.y,
                        selectedObject.transform.rotation.eulerAngles.z), Time.deltaTime * 1f);
            }
        }
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
