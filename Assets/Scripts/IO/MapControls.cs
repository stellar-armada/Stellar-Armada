using SpaceCommander.IO;
using SpaceCommander.Scenarios;
using UnityEngine;
using Zinnia.Data.Type.Transformation.Conversion;

public class MapControls : MonoBehaviour
{
    private bool isActive = false;

    Transform leftController;
    Transform rightController;
    [SerializeField] bool leftGripPressed;
    [SerializeField] bool rightGripPressed;

    private Vector3 leftPos;
    private Vector3 rightPos;
    private Vector3 leftPosPrev;
    private Vector3 rightPosPrev;

    float scaleFactor = 1f;

    bool invertControl = true;
    
    [SerializeField] private float scaleMin = 1f;
    [SerializeField] private float scaleMax = 100f;
    [SerializeField] private float startScale = 50f;

    void Start()
    {
        InputManager.instance.OnLeftSecondaryTrigger += HandleLeftInput;
        InputManager.instance.OnRightSecondaryTrigger += HandleRightInput;
        leftController = InputManager.instance.leftHand;
        rightController = InputManager.instance.rightHand;
        EnvironmentParent.instance.transform.localScale = Vector3.one * startScale;
    }

    void HandleLeftInput(bool on)
    {
        leftGripPressed = on;
        CheckState();
    }

    void HandleRightInput(bool on)
    {
        rightGripPressed = on;
        CheckState();
    }

    void CheckState()
    {
        // Both are pressed and they weren't before, so activate logic
        if (leftGripPressed && rightGripPressed && !isActive)
        {
            if (invertControl)
            {
                StartTransformationInverted();
            }
            else
            {
                StartTransformation();
            }

            isActive = true;
        }
        else if (isActive)
        {
            isActive = false;
            // Any cleanup logic can happen here
        }
    }

    void StartTransformation()
    {
        // Unparent scene root from scene parent
        MapTransformRoot.instance.transform.SetParent(null, true);

        // Calculate midpoint

        Vector3 midpoint = (leftController.position + rightController.position) / 2f;

        // Place scene parent at midpoint
        MapParent.instance.transform.position = midpoint;

        // reparent 
        MapTransformRoot.instance.transform.SetParent(MapParent.instance.transform, true);
    }

    void StartTransformationInverted()
    {
        // Unparent scene root from scene parent
        EnvironmentTransformRoot.instance.transform.SetParent(null, true);

        // Calculate midpoint

        Vector3 midpoint = (leftController.position + rightController.position) / 2f;

        // Place scene parent at midpoint
        EnvironmentParent.instance.transform.position = midpoint;

        // reparent 
        EnvironmentTransformRoot.instance.transform.SetParent(EnvironmentParent.instance.transform, true);
    }

    void Update()
    {
        //current position of controllers
        leftPos = leftController.position;
        rightPos = rightController.position;

        if (leftGripPressed && rightGripPressed)
        {
            if (invertControl)
            {
                TwoHandDragInverted();
                RotateInverted();
                ScaleInverted();
            }
            else
            {
                TwoHandDrag();
                Rotate();
                Scale();
            }
        }

        //previous position of controllers, to be used in the next frame
        leftPosPrev = leftController.transform.position;
        rightPosPrev = rightController.transform.position;
    }

    void TwoHandDragInverted()
    {
        //get middle position of hands
        Vector3 midPos = (leftPos + rightPos) / 2f;
        Vector3 prevMidPos = (leftPosPrev + rightPosPrev) / 2f;
        Vector3 distance = prevMidPos - midPos;
        EnvironmentTransformRoot.instance.transform.Translate(distance, Space.World);
    }

    void RotateInverted()
    {
        //project on XZ plane to restric rotation to flat table
        Vector3 dir = rightPos - leftPos;
        Vector3 prevDir = rightPosPrev - leftPosPrev;

        //center of hand pos
        Vector3 center = (leftPos + rightPos) / 2f;

        float angle = Vector3.Angle(dir, prevDir);

        //calculate direction of rotation
        Vector3 cross = Vector3.Cross(prevDir, dir);

        //perform rotation
        EnvironmentParent.instance.transform.RotateAround(center, cross, -angle);
    }

    void ScaleInverted()
    {
        //distance between hands 
        float dist = Vector3.Distance(leftPos, rightPos);
        float prevDist = Vector3.Distance(leftPosPrev, rightPosPrev);

        //scale factor based on difference in hand distance
        float newScale = prevDist / dist; // invert

        //convert middle position of hands from global to local space
        Vector3 midPosPreScale = (rightPosPrev + leftPosPrev) / 2f;
        Vector3 midPosLocal = EnvironmentParent.instance.transform.InverseTransformPoint(midPosPreScale);
        
        float currentScale = EnvironmentParent.instance.transform.localScale.x;
        
        //apply scale to model
        EnvironmentParent.instance.transform.localScale = Vector3.one * Mathf.Clamp(currentScale * (newScale * scaleFactor), scaleMin, scaleMax);
        
        //convert local position back to global and perform corrective translation
      // Vector3 midPosPostScale = EnvironmentParent.instance.transform.TransformPoint(midPosLocal);
      // Vector3 distance = (midPosPreScale - midPosPostScale);
      // EnvironmentParent.instance.transform.Translate(-distance, Space.World);
    }

    private void TwoHandDrag()
    {
        //get middle position of hands
        Vector3 midPos = (leftPos + rightPos) / 2f;
        Vector3 prevMidPos = (leftPosPrev + rightPosPrev) / 2f;
        Translate(prevMidPos, midPos);
    }

    private void Translate(Vector3 startPos, Vector3 endPos)
    {
        Vector3 distance = endPos - startPos;
        MapParent.instance.transform.Translate(distance, Space.World);
    }

    private void Rotate()
    {
        //project on XZ plane to restric rotation to flat table
        Vector3 dir = rightPos - leftPos;
        Vector3 prevDir = rightPosPrev - leftPosPrev;

        //center of hand pos
        Vector3 center = (leftPos + rightPos) / 2f;

        float angle = Vector3.Angle(dir, prevDir);

        //calculate direction of rotation
        Vector3 cross = Vector3.Cross(prevDir, dir);

        //perform rotation
        MapParent.instance.transform.RotateAround(center, cross, angle);
    }

    private void Scale()
    {
        //distance between hands 
        float dist = Vector3.Distance(leftPos, rightPos);
        float prevDist = Vector3.Distance(leftPosPrev, rightPosPrev);

        //scale factor based on difference in hand distance
        float scaleFactor = dist / prevDist;

        //convert middle position of hands from global to local space
        Vector3 midPosPreScale = (rightPosPrev + leftPosPrev) / 2f;
        Vector3 midPosLocal = MapParent.instance.transform.InverseTransformPoint(midPosPreScale);

        //apply scale to model
        MapParent.instance.transform.localScale *= scaleFactor;

        //convert local position back to global and perform corrective translation
        Vector3 midPosPostScale = MapParent.instance.transform.TransformPoint(midPosLocal);
        Translate(midPosPostScale, midPosPreScale);
    }
}