using StellarArmada.IO;
using UnityEngine;

namespace StellarArmada.Levels
{
    // Local player's minimap controller
    // Part of the local player object in our MatchPlayer prefab
    // TO-DO: Abstract grip => generic OnButtonDown (in case we want functionality on a different button)
    public class MiniMapController : MonoBehaviour
    {
        public static MiniMapController instance;

        public bool isActive = false;

        Transform leftController;
        Transform rightController;
        
        [SerializeField] bool leftGripPressed;
        [SerializeField] bool rightGripPressed;

        // Local variables for reuse
        private Vector3 leftPos;
        private Vector3 rightPos;
        private Vector3 leftPosPrev;
        private Vector3 rightPosPrev;
        private Transform miniMapTransformRoot;
        private Transform sceneRoot;
        private Transform miniMapTransform;
        private MiniMap miniMap;
        
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            // Event subscriptions are handled in Start to avoid race conditions with singleton inits
            InputManager.instance.OnLeftGrip += HandleLeftInput;
            InputManager.instance.OnRightGrip += HandleRightInput;
            leftController = InputManager.instance.leftHand;
            rightController = InputManager.instance.rightHand;
            
            // Set local references
            miniMapTransformRoot = MiniMapTransformRoot.instance.transform;
            miniMap = MiniMap.instance;
            miniMapTransform = MiniMap.instance.transform;
            sceneRoot = SceneRoot.instance.transform;
            
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
            else if (isActive) isActive = false;
        }

        // Since we're transforming the mini map by the space between our controllers
        // And not just scaling from the center of the map
        // We do some reparenting jiggery to make the math easier in the loop
        void StartTransformation()
        {
            // Put the transform root into the scene root if it wasn't already
            miniMapTransformRoot.SetParent(sceneRoot, true);
            // Unparent the minimap from the minimap transform root
           miniMapTransform.SetParent(sceneRoot, true);

            // Calculate midpoint
            Vector3 midpoint = (leftController.localPosition + rightController.localPosition) / 2f;

            // Place the minimap transform root at the midpoint
            miniMapTransformRoot.localPosition = midpoint;
            
            // Flatten out rotation so our rotations are less weird in the update loop
            miniMapTransformRoot.localRotation = Quaternion.identity;

            // reparent the minimap
            miniMapTransform.SetParent(miniMapTransformRoot, true);
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

        // Translate the minimap around
        private void TwoHandDrag()
        {
            Vector3 center = (leftPos + rightPos) / 2f;
            miniMapTransformRoot.localPosition = center;
        }

        // Rotate the minimap
        private void Rotate()
        {
            //project on XZ plane to restrict rotation to flat plane
            Vector3 dir = rightPos - leftPos;
            Vector3 prevDir = rightPosPrev - leftPosPrev;

            // Calculate the angle
            float angle = Vector3.Angle(dir, prevDir);

            //calculate direction of rotation by crossing 
            Vector3 cross = Vector3.Cross(prevDir, dir);
            
            //perform rotation
            miniMapTransformRoot.Rotate(cross, angle);
        }

        // Scale the minimap
        private void Scale()
        {
            //distance between hands 
            float dist = Vector3.Distance(leftPos, rightPos);
            float prevDist = Vector3.Distance(leftPosPrev, rightPosPrev);

            //scale factor based on difference in hand distance
            float scaleFactor = dist / prevDist;

            //apply scale to model
            miniMapTransformRoot.localScale *= scaleFactor;
            miniMapTransformRoot.localScale = Vector3.one * 
                                              Mathf.Clamp(miniMapTransformRoot.localScale.x, miniMap.minScale, miniMap.maxScale);
        }
    }
}