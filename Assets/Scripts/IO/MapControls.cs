using SpaceCommander.IO;
using SpaceCommander.Scenarios;
using UnityEngine;

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
	void Start()
    {
        InputManager.instance.LeftSecondaryTrigger += HandleLeftInput;
        InputManager.instance.RightSecondaryTrigger += HandleRightInput;
        leftController = InputManager.instance.leftHand;
        rightController = InputManager.instance.rightHand;
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
            StartTransformation();
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
        MapParent.instance.transform.SetParent(null, true);

        // Calculate midpoint

        Vector3 midpoint = (leftController.position + rightController.position) / 2f;

        // Place scene parent at midpoint
        MapTransformRoot.instance.transform.position = midpoint;
        
        // reparent 
        MapParent.instance.transform.SetParent(MapTransformRoot.instance.transform, true);

    }

    void Update ()
    {
        //current position of controllers
        leftPos = leftController.position;
        rightPos = rightController.position;

        if(leftGripPressed && rightGripPressed){
            TwoHandDrag();
            Rotate();
            Scale();
        }

        //previous position of controllers, to be used in the next frame
        leftPosPrev = leftController.transform.position;
        rightPosPrev = rightController.transform.position;
    }

    private void TwoHandDrag() {
        //get middle position of hands
        Vector3 midPos = (leftPos + rightPos) / 2f;
        Vector3 prevMidPos = (leftPosPrev + rightPosPrev) / 2f;
        Translate(prevMidPos, midPos);
    }

    private void Translate(Vector3 startPos, Vector3 endPos) {
        Vector3 distance = endPos - startPos;
        MapTransformRoot.instance.transform.Translate(distance, Space.World);
    }

    private void Rotate() {
        //project on XZ plane to restric rotation to flat table
        Vector3 dir = rightPos - leftPos;
        Vector3 prevDir = rightPosPrev - leftPosPrev;

        //center of hand pos
        Vector3 center = (leftPos + rightPos) / 2f;

        float angle = Vector3.Angle(dir, prevDir);

        //calculate direction of rotation
        Vector3 cross = Vector3.Cross(prevDir, dir);

        //perform rotation
        MapTransformRoot.instance.transform.RotateAround(center, cross, angle);
    }

    private void Scale() {
        //distance between hands 
        float dist = Vector3.Distance(leftPos, rightPos);
        float prevDist = Vector3.Distance(leftPosPrev, rightPosPrev);

        //scale factor based on difference in hand distance
        float scaleFactor = dist / prevDist;

        //convert middle position of hands from global to local space
        Vector3 midPosPreScale = (rightPosPrev + leftPosPrev) / 2f;
        Vector3 midPosLocal = MapTransformRoot.instance.transform.InverseTransformPoint(midPosPreScale);

        //apply scale to model
        MapTransformRoot.instance.transform.localScale *= scaleFactor;

        //convert local position back to global and perform corrective translation
        Vector3 midPosPostScale = MapTransformRoot.instance.transform.TransformPoint(midPosLocal);
        Translate(midPosPostScale, midPosPreScale);
    }



}
