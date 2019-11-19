using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    public static RTSCameraController instance;
    
    public Transform cameraParent;

    [SerializeField] private float moveSpeed = 10;

    private bool cameraIsPanning;

    public bool cameraLocked = false;

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    void Awake()
    {
        instance = this;
    }

    
    void Update()
    {
        // Assuming that the camera is panning, unless it's not
        cameraIsPanning = false;

        // Keyboard controls for debug, replace this with event driven DPad later

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(horizontal) > .001f)
        {
            cameraParent.localPosition += horizontal * moveSpeed * Vector3.left;
            cameraIsPanning = true;
        }
        if (Mathf.Abs(vertical) > .001f)
        {
            cameraParent.localPosition += vertical * moveSpeed * Vector3.forward;
            cameraIsPanning = true;
        }

        // Calculate distance from the camera to our flagship (local space 0)
        float cameraDistance = Vector3.Distance(Vector3.zero, cameraParent.localPosition);
        // If the camera isn't being moved and it's not zero'd, move it back to 0 at fixed move speed
        if (!cameraIsPanning && !cameraLocked && cameraDistance > .01f)
        {
            Vector3 localPos = cameraParent.localPosition;
            // Get the direction to move by multiplying the normalized position by the inverted movement speed
            Vector3 directiontoMove = (localPos.normalized * -moveSpeed);

            // If it's gonna overshoot the intended distance, clamp it to the zero vector in 2D space
            if (directiontoMove.magnitude > cameraDistance)
                cameraParent.localPosition = new Vector3(0, 0, 0);

            // Otherwise, move it toward
            else
                cameraParent.localPosition =
                    new Vector3(localPos.x + directiontoMove.x,
                        0,
                        localPos.z + directiontoMove.z);
        }
    }
}