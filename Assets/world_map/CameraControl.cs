using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float zoomSpeed;
    public float moveSpeed;
    public float zoomMoveWeight;

    Vector3 lastMousePos;

    void Update()
    {
        transform.position += new Vector3(0, -Input.mouseScrollDelta.y * zoomSpeed, 0);

        Vector3 deltaPos = lastMousePos - Input.mousePosition;

        if (Input.GetKey(KeyCode.Mouse0))
            transform.position += new Vector3(
                deltaPos.x * moveSpeed * zoomMoveWeight * transform.position.y, 0, 
                deltaPos.y * moveSpeed * zoomMoveWeight * transform.position.y);

        lastMousePos = Input.mousePosition;
    }
}
