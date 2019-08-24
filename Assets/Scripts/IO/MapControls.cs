using StellarArmada.IO;
using StellarArmada.Scenarios;
using UnityEngine;

public class MapControls : MonoBehaviour
{
    public static MapControls instance;
    
    public bool isActive = false;

    Transform leftController;
    Transform rightController;
    [SerializeField] bool leftGripPressed;
    [SerializeField] bool rightGripPressed;

    private Vector3 leftPos;
    private Vector3 rightPos;
    private Vector3 leftPosPrev;
    private Vector3 rightPosPrev;

    float scaleFactor = 1f;
    
    [SerializeField] private float scaleMin = .0001f;
    [SerializeField] private float scaleMax = .001f;

    public delegate void MapChangeEventVector(Vector3 vector);

    public MapChangeEventVector OnScaleChanged;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        InputManager.instance.OnLeftGrip += HandleLeftInput;
        InputManager.instance.OnRightGrip += HandleRightInput;
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
        MapTransformRoot.instance.transform.SetParent(SceneRoot.instance.transform, true);
        // Unparent scene root from scene parent
        MiniMap.instance.transform.SetParent(SceneRoot.instance.transform, true);

        // Calculate midpoint

        Vector3 midpoint = (leftController.localPosition + rightController.localPosition) / 2f;

        // Place scene parent at midpoint
        MapTransformRoot.instance.transform.localPosition = midpoint;
        MapTransformRoot.instance.transform.localRotation = Quaternion.identity;

        // reparent 
        MiniMap.instance.transform.SetParent(MapTransformRoot.instance.transform, true);
    }

    void Update()
    {
        //current position of controllers
        leftPos = leftController.localPosition;
        rightPos = rightController.localPosition;

        if (isActive)
        {
            TwoHandDrag();
                Rotate();
                Scale();
        }

        //previous position of controllers, to be used in the next frame
        leftPosPrev = leftController.localPosition;
        rightPosPrev = rightController.localPosition;
    }

    private void TwoHandDrag()
    {
        Vector3 center = (leftPos + rightPos) / 2f;
        MapTransformRoot.instance.transform.localPosition = center;
    }


    private void Rotate()
    {
        //project on XZ plane to restric rotation to flat table
        Vector3 dir = rightPos - leftPos;
        Vector3 prevDir = rightPosPrev - leftPosPrev;

        float angle = Vector3.Angle(dir, prevDir);

        //calculate direction of rotation
        Vector3 cross = Vector3.Cross(prevDir, dir);
        //perform rotation
        MapTransformRoot.instance.transform.Rotate(cross, angle);
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

        Transform t = MapTransformRoot.instance.transform;
        
        //apply scale to model
        t.localScale *= scaleFactor;
        t.localScale =  Vector3.one * Mathf.Clamp(t.localScale.x, MiniMap.instance.minScale, MiniMap.instance.maxScale);
        OnScaleChanged?.Invoke(t.localScale);
    }
}