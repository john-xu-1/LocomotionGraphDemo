using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minCameraSize = 5, maxCameraSize = float.MaxValue;
    public float scrollScale = 1;
    public float movementScale = 1;
    private Vector2 previousMoustPos;
    public bool isUIActive = false;

    // Update is called once per frame
    void Update()
    {

        if (!isUIActive)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.mouseScrollDelta.y * scrollScale, minCameraSize, maxCameraSize);

            if (Input.GetMouseButton(1))
            {
                Vector3 mouseDelta = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - previousMoustPos;
                transform.position = transform.position - mouseDelta * movementScale;
            }

            previousMoustPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
    }
}
