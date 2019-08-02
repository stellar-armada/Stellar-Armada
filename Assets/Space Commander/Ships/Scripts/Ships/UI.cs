using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.Ships
{
    public class UI : MonoBehaviour
    {
        [SerializeField] Image frame;
        [SerializeField] private Transform uiRoot;
        [SerializeField] private CanvasGroup uiCanvasGroup;
        private Ship ship;
        private Transform cameraTransform;
        void Awake()
        {
            ship = GetComponent<Ship>();
            cameraTransform = Camera.main.transform;
            LookAtMainCamera();
        }
        
        void LateUpdate ()
        {
            LookAtMainCamera();
        }

        void LookAtMainCamera()
        {
            // TO-DO: Fix so that always facing camera, but also oriented nice vetically, not rotated with ship
            Vector3 targetPostition = new Vector3( cameraTransform.position.x, 
                uiRoot.position.y, 
                cameraTransform.position.z ) ;
            uiRoot.LookAt( targetPostition ) ;
            
            
            
        }
        
        public void ShowUI()
        {
            uiCanvasGroup.alpha = 1f;
        }
        
        public void HideUI()
        {
            uiCanvasGroup.alpha = 0f;
        }
        
        // Show frame
        public void ShowFrame()
        {
            frame.enabled = true;
        }
        
        // Hide frame
        public void HideFrame()
        {
            frame.enabled = false;
        }

        // Set frame color

        public void SetFrameColor(Color color)
        {
            frame.color = color;
        }
        
        // Set flag / icon for ship team identification
    }
}
