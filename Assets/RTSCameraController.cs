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

    [SerializeField] private CanvasGroup group;

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    void Awake()
    {
        instance = this;
        group.gameObject.SetActive(false);

    }

    public void FadeInHud()
    {
        StartCoroutine((IFadeIn()));
    }

    IEnumerator IFadeIn()
        {
            float timer = 0f;
            group.gameObject.SetActive(true);
            do
            {
                timer += Time.deltaTime;
                group.alpha = Mathf.Lerp(0, 1, timer);
                yield return null;
            }
            while (timer <= 1) ;
        }
    
    void Update()
    {
        // Assuming that the camera is panning, unless it's not
        cameraIsPanning = false;

        // Keyboard controls for debug, replace this with event driven DPad later

        float horizontal = SimpleInput.GetAxis("Horizontal");
        float vertical = SimpleInput.GetAxis("Vertical");
        
        if (Mathf.Abs(horizontal) > .001f)
        {
            cameraParent.localPosition += horizontal * moveSpeed * Vector3.back;
            cameraIsPanning = true;
        }
        if (Mathf.Abs(vertical) > .001f)
        {
            cameraParent.localPosition += vertical * moveSpeed * Vector3.right;
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