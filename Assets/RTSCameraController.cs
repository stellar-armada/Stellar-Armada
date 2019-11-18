using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [SerializeField] Transform cameraParent;

    [SerializeField] private float moveSpeed = 10;

    private bool cameraIsPanning;

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
    
    void Update()
    {
        // Assuming that the camera is panning, unless it's not
        cameraIsPanning = false;

        // Keyboard controls for debug, replace this with event driven DPad later
        if (Input.GetKey(KeyCode.S))
        {
            cameraParent.localPosition += Vector3.left * moveSpeed;
            cameraIsPanning = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            cameraParent.localPosition += Vector3.right * moveSpeed;
            cameraIsPanning = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            cameraParent.localPosition += Vector3.forward * moveSpeed;
            cameraIsPanning = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            cameraParent.localPosition += Vector3.back * moveSpeed;
            cameraIsPanning = true;
        }


        // Calculate distance from the camera to our flagship (local space 0)
        float cameraDistance = Vector3.Distance(Vector3.zero, cameraParent.localPosition);
        
        Debug.Log("Camera distance: " + cameraDistance);

        // If the camera isn't being moved and it's not zero'd, move it back to 0 at fixed move speed
        if (!cameraIsPanning && cameraDistance > .01f)
        {
            // Get the direction to move by multiplying the normalized position by the inverted movement speed
            Vector3 directiontoMove = (cameraParent.localPosition.normalized * -moveSpeed);

            // If it's gonna overshoot the intended distance, clamp it to the zero vector in 2D space
            if (directiontoMove.magnitude > cameraDistance)
                cameraParent.localPosition = new Vector3(0, 0, 0);

            // Otherwise, move it toward
            else
                cameraParent.localPosition =
                    new Vector3(cameraParent.localPosition.x + directiontoMove.x,
                        0,
                        cameraParent.localPosition.z + directiontoMove.z);
        }
    }
}